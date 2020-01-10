const { EventEmitter } = require('events')

const Lock = require('../library/lock')

const Socket = require('../models/socket')

let _io = null

class PlayerEvents extends EventEmitter {
  constructor (gameId) {
    super()

    this._gameId = gameId

    this.emit = this.emit.bind(this)

    this.onPlayerJoined = this.onPlayerJoined.bind(this)
    this.onPlayerDisconnected = this.onPlayerDisconnected.bind(this)
    this.onPlayerLeft = this.onPlayerLeft.bind(this)

    this._updatingSockets = new Lock()

    Socket.find({ game: this._gameId })
      .then(async sockets => {
        this._sockets = sockets
        this._updatingSockets.release()
      })

    Socket.watch([
      { $match: { game: gameId } }
    ]).on('change', async data => {
      await this._updatingSockets.acquire()
      if (data.operationType === 'insert') {
        this._sockets.push(data.fullDocument)
      } else if (data.operationType === 'delete') {
        const i = this._sockets.findIndex(s => s.id === data.documentKey._id)
        this._sockets.splice(i, 1)
      }
      this._updatingSockets.release()
    })
  }

  async emit (eventName, data) {
    await this._updatingSockets.acquire()
    super.emit(eventName, data)
    if (_io && this._sockets) {
      this._sockets.forEach(({ socketId }) => {
        _io.to(socketId).emit(eventName, data)
      })
    }
    this._updatingSockets.release()
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
}

PlayerEvents.setIo = io => { _io = io }

module.exports = PlayerEvents
