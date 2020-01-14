const SocketEmitter = require('./socketEmitter')

class PlayerEvents extends SocketEmitter {
  constructor (gameId) {
    super(gameId)

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
    return this.emit('playerMoved', { user, location })
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
}

module.exports = PlayerEvents
