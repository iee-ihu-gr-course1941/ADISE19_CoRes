const config = require('../config.json')

const Game = require('../models/game')

const SocketManager = require('../socket-io/socketManager')
const SocketWatcher = require('../socket-io/socketWatcher')

const GameHolder = require('./gameHolder')
const GameError = require('./gameError')

class GameManager {
  constructor (user) {
    this._user = user
    this._gameHolder = null

    this.create = this.create.bind(this)
    this.join = this.join.bind(this)
    this.leave = this.leave.bind(this)
    this.current = this.current.bind(this)
    this.getGameHolder = this.getGameHolder.bind(this)
  }

  async init () {
    // Try to load previous game
    const lastGameId = this._user && this._user.lastGame
    if (lastGameId) {
      const game = await GameHolder.getGameHolder(lastGameId)

      if (game) {
        // Check game.players for current user
        const players = game.getJSON().players
        const found = players.find(p => {
          const pUserId = p.user.toString()
          const sUserId = this._user._id
          return pUserId === sUserId
        })

        if (found) {
          this._gameHolder = game
        }
      }
    }
  }

  async create (seats) {
    const self = this

    if (self._gameHolder) {
      throw new GameError('GameManager already has a game attached')
    }

    // Initialize Game
    const player = {
      user: self._user._id,
      name: self._user.username,
      balance: config.game.initialBalance,
      position: 0,
      duplicateRolls: 0,
      jailed: false
    }
    let game = new Game({
      players: [player],
      seats,
      status: 'waitingPlayers',
      properties: config.prices.properties
    })
    game = await game.save()

    self._gameHolder = await GameHolder.getGameHolder(game.id)

    self._gameHolder.getPlayerEvents().onPlayerJoined(player)
    SocketManager.updateSocketsGameFromUser(self._user._id, game.id)

    return self._gameHolder.getJSON()
  }

  async join (gameId) {
    const self = this

    if (self._gameHolder) {
      throw new Error('GameManager already has a game attached')
    }

    // Find game in DB
    const game = await Game.findById(gameId)

    // Check if game was found
    if (!game) {
      throw new GameError('Game not found')
    }

    self._gameHolder = await GameHolder.getGameHolder(gameId)

    let player

    // Check if user is in game.players
    const playerIndex = game.players.findIndex(p => p.user.toString() === self._user._id)
    if (playerIndex >= 0) { // Reconnect if user was left in that game
      if (game.status === 'ended') {
        throw new GameError('Game has ended')
      }

      if (game.status === 'running') {
        const disconnectTime = new Date(self._user.disconnected).getTime()
        if (disconnectTime + config.game.msToReconnect < Date.now()) {
          // TODO: Player should have automatically been kicked from game by this time
          throw new GameError('You cannot reconnect to this game')
        }
      }

      player = game.players[playerIndex]
    } else { // Try to join game
      // Check for empty seats
      if (game.players.length > game.seats) {
        // TODO: Unexpected behavior
        throw new Error('Game has more players than expected')
      } else if (game.players.length >= game.seats) {
        throw new GameError('Game is full')
      }

      // Check game status
      if (game.status !== 'waitingPlayers') {
        throw new GameError('This game is not waiting for players to join')
      }

      // Add user to game.players
      player = {
        user: self._user._id,
        name: self._user.username,
        balance: config.game.initialBalance,
        position: 0,
        duplicateRolls: 0,
        jailed: false,
        index: [...Array(config.game.maxPlayers)].map((_, i) => i).find(i => !game.players.find(({ index }) => index === i))
      }
      game.players.push(player)

      if (game.players.length >= game.seats) {
        game.status = 'running'
        self._gameHolder.getGameEvents().onGameStarted()
      }

      await game.save()
    }

    self._gameHolder.getPlayerEvents().onPlayerJoined(player)
    SocketManager.updateSocketsGameFromUser(self._user._id, game.id)

    await self._gameHolder.update()

    return self._gameHolder.getJSON()
  }

  async leave () {
    const self = this

    if (!self._gameHolder) {
      throw new GameError('User is not playing any games')
    }

    // Find game in DB
    let game = await Game.findById(self._gameHolder.getJSON()._id)

    // Check if game was found
    if (!game) {
      // TODO: Unexpected behavior
      throw new Error('Game not found')
    }

    // Check if user is in game.players
    const found = game.players.find(p => p.user.toString() === self._user._id)
    if (!found) {
      // TODO: Unexpected behavior
      throw new Error('User is not playing in this game')
    }

    // Remove user from game.players
    game = await Game.findByIdAndUpdate(
      game.id,
      {
        $pull: {
          players: { user: self._user._id }
        }
      },
      {
        new: true,
        runValidators: true
      }
    )

    SocketManager.updateSocketsGameFromUser(self._user._id)
    self._gameHolder.getPlayerEvents().onPlayerLeft(self._user._id)

    if (game.players.length === 1 && game.status === 'running') {
      game = await Game.findByIdAndUpdate(
        game.id,
        {
          $set: {
            status: 'ended'
          }
        },
        {
          new: true,
          runValidators: true
        }
      )

      self._gameHolder.getGameEvents().onGameEnded()
    }

    await self._gameHolder.update()

    if (!game.players.length) {
      SocketWatcher.disposeSocketWatcher(game.id)
    }

    return self._gameHolder.getJSON()
  }

  current () {
    if (this._gameHolder) {
      SocketManager.updateSocketsGameFromUser(this._user._id, this._gameHolder._game._id)
      return this._gameHolder.getJSON()
    }
  }

  getGameHolder () {
    return this._gameHolder
  }
}

module.exports = GameManager
