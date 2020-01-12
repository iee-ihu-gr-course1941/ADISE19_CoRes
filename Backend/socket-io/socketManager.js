const { Types } = require('mongoose')

const Socket = require('../models/socket')

const checkForSocket = async data => {
  const socket = await Socket.findOne({
    sessionId: data.sessionId,
    socketId: data.socketId
  })
  return socket
}

// Connect
module.exports.getSocket = async data => {
  let socket = await checkForSocket(data)

  if (!socket) {
    socket = await new Socket(data).save()
  }

  return socket.toJSON()
}

// Disconnect
module.exports.deleteSocket = async socketId => {
  const result = await Socket.findOneAndRemove({ socketId })
  return result
}

// Logout
module.exports.deleteSocketsForSession = async sessionId => {
  const result = await Socket.findOneAndRemove({ sessionId })
  return result
}

module.exports.updateSocketsGameFromUser = async (userId, gameId) => {
  const result = await Socket.updateMany(
    { user: Types.ObjectId(userId) },
    gameId ? { $set: { game: gameId } } : { $unset: { game: '' } },
    {
      new: true,
      runValidators: true
    }
  )

  return result
}

module.exports.updateSocketsUserFromSession = async (sessionId, userId) => {
  const result = await Socket.updateMany(
    { sessionId },
    userId ? { $set: { user: Types.ObjectId(userId) } } : { $unset: { user: '' } },
    {
      new: true,
      runValidators: true
    }
  )

  return result
}
