const SocketEmitter = require('./socketEmitter')

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
}

module.exports = PlayerEvents
