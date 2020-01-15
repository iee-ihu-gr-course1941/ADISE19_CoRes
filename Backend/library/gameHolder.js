const { Types } = require('mongoose')

const Game = require('../models/game')

const GameEvents = require('../socket-io/gameEvents')
const PlayerEvents = require('../socket-io/playerEvents')
const PropertyEvents = require('../socket-io/propertyEvents')

class GameHolder {
  constructor (game) {
    if (!game) {
      throw new Error('game is required')
    }

    this._game = game
    // TODO: Subscribe to change stream

    this.getJSON = this.getJSON.bind(this)
    this.getGameEvents = this.getGameEvents.bind(this)
    this.getPlayerEvents = this.getPlayerEvents.bind(this)
    this.getPropertyEvents = this.getPropertyEvents.bind(this)
    this.update = this.update.bind(this)

    this._gameEvents = new GameEvents(this)
    this._playerEvents = new PlayerEvents(this)
    this._propertyEvents = new PropertyEvents(this)
  }

  getJSON () {
    return this._game.toJSON()
  }

  getGameEvents () {
    return this._gameEvents
  }

  getPlayerEvents () {
    return this._playerEvents
  }

  getPropertyEvents () {
    return this._propertyEvents
  }

  async update () {
    this._game = await Game.findById(this._game.id)
    return this
  }
}

GameHolder.getGameHolder = async gameId => {
  const game = await Game.findOne({
    _id: Types.ObjectId(gameId),
    status: { $ne: 'ended' }
  })

  if (game) {
    return new GameHolder(game)
  }
}

module.exports = GameHolder
