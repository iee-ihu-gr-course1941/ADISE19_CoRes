const config = require('../config.json')

const SocketEmitter = require('./socketEmitter')

const Game = require('../models/game')

const chanceCards = require('../library/chances')
const communityChestCards = require('../library/communityChests')

class PlayerEvents extends SocketEmitter {
  constructor (gameHolder) {
    super(gameHolder)

    this.onPlayerJoined = this.onPlayerJoined.bind(this)
    this.onPlayerDisconnected = this.onPlayerDisconnected.bind(this)
    this.onPlayerLeft = this.onPlayerLeft.bind(this)
    this.onPlayerRolledDice = this.onPlayerRolledDice.bind(this)
    this.onPlayerMoved = this.onPlayerMoved.bind(this)
    this.onPlayerTurnChanged = this.onPlayerTurnChanged.bind(this)
    this.onPlayerPlaysAgain = this.onPlayerPlaysAgain.bind(this)
    this.onPlayerPassedFromGo = this.onPlayerPassedFromGo.bind(this)
    this.onPlayerGotPaid = this.onPlayerGotPaid.bind(this)
    this.onPlayerPaid = this.onPlayerPaid.bind(this)
    this.onPlayerBalanceChanged = this.onPlayerBalanceChanged.bind(this)
    this.onPlayerSteppedOnChance = this.onPlayerSteppedOnChance.bind(this)
    this.onPlayerSteppedOnCommunityChest = this.onPlayerSteppedOnCommunityChest.bind(this)
    this.onPlayerSteppedOnTax = this.onPlayerSteppedOnTax.bind(this)
  }

  onPlayerJoined (player) {
    return this.emit('playerJoined', { player })
  }

  onPlayerDisconnected (user) {
    return this.emit('playerDisconnected', { user })
  }

  onPlayerLeft (user) {
    return this.emit('playerLeft', { user })
  }

  onPlayerRolledDice (user, dice) {
    return this.emit('playerRolledDice', { user, dice })
  }

  onPlayerMoved (user, location) {
    const emitResult = this.emit('playerMoved', { user, location })

    // Check for chance
    const chances = [7, 22, 36]
    if (chances.includes(location)) {
      this.onPlayerSteppedOnChance(user)
    }

    // Check for Community Chest
    const communityChests = [2, 17, 33]
    if (communityChests.includes(location)) {
      this.onPlayerSteppedOnCommunityChest(user)
    }

    // Check for tax
    const taxes = [4, 38]
    if (taxes.includes(location)) {
      const index = config.prices.taxes.findIndex(t => t.location === location)
      this.onPlayerSteppedOnTax(user, index)
    }

    return emitResult
  }

  onPlayerTurnChanged (user) {
    return this.emit('playerTurnChanged', { user })
  }

  onPlayerPlaysAgain (user) {
    return this.emit('playerPlaysAgain', { user })
  }

  onPlayerPassedFromGo (user) {
    this.emit('playerPassedFromGo', { user })
  }

  onPlayerGotPaid (user, money) {
    this.emit('playerGotPaid', { user, money })
  }

  onPlayerPaid (user, money) {
    this.emit('playerPaid', { user, money })
  }

  onPlayerBalanceChanged (user, balance) {
    this.emit('playerBalanceChanged', { user, balance })
  }

  async onPlayerSteppedOnChance (user) {
    if (chanceCards.length) {
      const chance = Math.floor(Math.random() * chanceCards.length)
      const card = chanceCards[chance]
      const emitResult = this.emit('playerSteppedOnChance', { user, card: card.text })
      card.action(user, this._gameHolder)
      return emitResult
    }
  }

  async onPlayerSteppedOnCommunityChest (user) {
    if (communityChestCards.length) {
      const communityChest = Math.floor(Math.random() * communityChestCards.length)
      const card = communityChestCards[communityChest]
      const emitResult = this.emit('playerSteppedOnCommunityChest', { user, card: card.text })
      card.action(user, this._gameHolder)
      return emitResult
    }
  }

  async onPlayerSteppedOnTax (user, tax) {
    const emitResult = this.emit('playerSteppedOnTax', { user, tax })
    const currentGame = this._gameHolder.getJSON()
    const player = currentGame.players.find(p => p.user.toString() === user.toString())
    if (player.balance > config.prices.taxes[tax].price) {
      const game = await Game.findById(this._gameId)
      const gamePlayer = game.players.find(p => p.user.toString() === user.toString())
      gamePlayer.balance -= config.prices.taxes[tax].price
      this._gameHolder.getPlayerEvents().onPlayerPaid(player.user, config.prices.taxes[tax].price)
      this._gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      await game.save()
      await this._gameHolder.update()
    }
    return emitResult
  }
}

module.exports = PlayerEvents
