const config = require('../config.json')

const Game = require('../models/game')

const { getGame } = require('./gameHolder')
const GameError = require('./gameError')

class GameManager {
  constructor (user, gameHolder) {
    this._user = user
    this._gameHolder = gameHolder // nullable

    this.create = this.create.bind(this)
    this.join = this.join.bind(this)
    this.leave = this.leave.bind(this)
    this.current = this.current.bind(this)
  }

  async create (seats) {
    const self = this

    if (self._gameHolder) {
      throw new GameError('GameManager already has a game attached')
    }

    // Initialize Game
    let game = new Game({
      players: [{
        user: self._user._id,
        balance: config.game.initialBalance,
        position: 0,
        duplicateRolls: 0,
        jailed: false
      }],
      seats,
      status: 'waitingPlayers',
      properties: config.prices.properties
    })
    game = await game.save()

    self._gameHolder = await getGame(game.id)

    // TODO: Trigger game join event

    return self._gameHolder
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
      game.players.push({
        user: self._user._id,
        balance: config.game.initialBalance,
        position: 0,
        duplicateRolls: 0,
        jailed: false
      })
      await game.save()
    }

    self._gameHolder = await getGame(game.id, { update: true })

    // TODO: Trigger game join event

    return self._gameHolder
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

    // TODO: Trigger game leave event

    return getGame(game.id, { update: true })
  }

  current () {
    return this._gameHolder && this._gameHolder.getJSON()
  }
}

module.exports = GameManager
