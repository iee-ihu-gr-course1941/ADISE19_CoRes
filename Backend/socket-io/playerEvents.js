const SocketEmitter = require('./socketEmitter')

class PlayerEvents extends SocketEmitter {
  constructor (gameId) {
    super(gameId)

    this.onPlayerJoined = this.onPlayerJoined.bind(this)
    this.onPlayerDisconnected = this.onPlayerDisconnected.bind(this)
    this.onPlayerLeft = this.onPlayerLeft.bind(this)
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
}

module.exports = PlayerEvents
