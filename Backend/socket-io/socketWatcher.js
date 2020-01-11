const { EventEmitter } = require('events')

const Lock = require('../library/lock')

const Socket = require('../models/socket')

const socketWatchers = { }

class SocketWatcher extends EventEmitter {
  constructor (gameId) {
    super()

    this._sockets = new Map()

    this._updatingSockets = new Lock()

    this._updatingSockets.acquire()
      .then(() => Socket.find({ game: gameId }))
      .then(async sockets => {
        sockets.forEach(s => this._sockets.set(s.id, s))
        this._updatingSockets.release()
      })

    this._cursor = Socket.watch([], { fullDocument: 'updateLookup' }) // TODO: Optimize

    this._cursor.on('change', async data => {
      await this._updatingSockets.acquire()
      try {
        if (data.operationType === 'insert') {
          const id = data && data.fullDocument && data.fullDocument.game && data.fullDocument.game._id && data.fullDocument.game._id.toString()
          if (id === gameId) {
            const id = data.fullDocument._id.toString()
            this._sockets.set(id, data.fullDocument)
          }
        } else if (data.operationType === 'delete') {
          const id = data && data.documentKey && data.documentKey._id && data.documentKey._id.toString()
          if (this._sockets.has(id)) {
            this._sockets.delete(id)
          }
        }
      } catch (err) {
        console.log(err)
      }
      this._updatingSockets.release()
    })

    this._cursor.on('error', err => {
      console.error(err)
    })
  }

  getSockets () {
    return this._sockets
  }

  updatingSockets () {
    return this._updatingSockets
  }
}

SocketWatcher.getSocketWatcher = gameId => {
  if (!socketWatchers[gameId]) {
    socketWatchers[gameId] = new SocketWatcher(gameId)
  }

  return socketWatchers[gameId]
}

SocketWatcher.disposeSocketWatcher = gameId => {
  if (socketWatchers[gameId]) {
    socketWatchers[gameId]._cursor.close()
    delete socketWatchers[gameId]
  }
}

module.exports = SocketWatcher
