const mongoose = require('mongoose')

const Schema = mongoose.Schema

const userSchema = new Schema(
  {
    username: {
      type: String,
      required: true,
      unique: true
    },
    passwordHash: {
      type: String,
      required: true
    },
    lastGame: { // TODO: Implement reconnect
      type: Schema.Types.ObjectId,
      required: false,
      ref: 'Game'
    },
    disconnected: { // TODO: Implement disconnection detection
      type: Date,
      required: false
    }
  },
  { timestamps: true }
)

module.exports = mongoose.model('User', userSchema)
