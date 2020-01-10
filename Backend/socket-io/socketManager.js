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
