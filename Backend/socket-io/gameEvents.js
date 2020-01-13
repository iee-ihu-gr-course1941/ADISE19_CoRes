const SocketEmitter = require('./socketEmitter')

class GameEvents extends SocketEmitter {
  constructor (gameId) {
    super(gameId)

    this.onGameStarted = this.onGameStarted.bind(this)
    this.onGameEnded = this.onGameEnded.bind(this)
  }

  onGameStarted (firstPlayer) {
    return this.emit('gameStarted', { firstPlayer })
  }

  onGameEnded () {
    return this.emit('gameEnded', {})
  }
}

module.exports = GameEvents
