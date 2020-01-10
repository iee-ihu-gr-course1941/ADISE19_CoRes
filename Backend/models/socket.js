const mongoose = require('mongoose')

const Schema = mongoose.Schema

const socketSchema = new Schema(
  {
    game: {
      type: Schema.Types.ObjectId,
      required: false,
      ref: 'Game'
    },
    user: {
      type: Schema.Types.ObjectId,
      required: false,
      ref: 'User'
    },
    sessionId: {
      type: String,
      required: true
    },
    socketId: {
      type: String,
      required: true
    }
  }
)

module.exports = mongoose.model('Sockets', socketSchema)
