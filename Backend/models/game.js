const config = require('../config.json')

const mongoose = require('mongoose')

const Schema = mongoose.Schema

const gameSchema = new Schema(
  {
    players: [{
      user: {
        type: Schema.Types.ObjectId,
        required: true,
        ref: 'User'
      },
      balance: {
        type: Number,
        required: true
      },
      position: {
        type: Number,
        required: true
      },
      duplicateRolls: {
        type: Number,
        required: true
      },
      jailed: {
        type: Boolean,
        required: true
      },
      _id: false
    }],
    properties: [{
      name: {
        type: String,
        required: true
      },
      owner: {
        type: Schema.Types.ObjectId,
        required: false,
        ref: 'User'
      },
      mortgaged: {
        type: Boolean,
        required: false
      }
    }],
    seats: {
      type: Number,
      required: true
    },
    status: {
      type: String,
      enum: config.game.status,
      required: true
    },
    currentPlayer: {
      type: Schema.Types.ObjectId,
      required: false,
      ref: 'User'
    }
  },
  { timestamps: true }
)

module.exports = mongoose.model('Game', gameSchema)
