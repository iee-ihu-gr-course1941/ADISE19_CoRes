const { EventEmitter } = require('events')

const SocketWatcher = require('./socketWatcher')

let _io = null

class SocketEmitter extends EventEmitter {
  constructor (gameId) {
    super()

    this._gameId = gameId
    this.emit = this.emit.bind(this)
    this._socketWatcher = SocketWatcher.getSocketWatcher(gameId)
  }

  async emit (eventName, data) {
    console.log(eventName, data)
    await this._socketWatcher.updatingSockets().acquire()
    super.emit(eventName, data)
    const sockets = this._socketWatcher.getSockets()
    if (_io && sockets) {
      sockets.forEach(({ socketId }) => {
        _io.to(socketId).emit(eventName, data)
      })
    }
    this._socketWatcher.updatingSockets().release()
  }
}

SocketEmitter.setIo = io => { _io = io }

module.exports = SocketEmitter
