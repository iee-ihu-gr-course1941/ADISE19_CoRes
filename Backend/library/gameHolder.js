const Game = require('../models/game')

const PlayerEvents = require('../socket-io/playerEvents')

class GameHolder {
  constructor (game) {
    if (!game) {
      throw new Error('game is required')
    }

    this._game = game
    this._playerEvents = new PlayerEvents(game._id)
    // TODO: Subscribe to change stream

    this.getJSON = this.getJSON.bind(this)
    this.getPlayerEvents = this.getPlayerEvents.bind(this)
    this.update = this.update.bind(this)
  }

  getJSON () {
    return this._game.toJSON()
  }

  getPlayerEvents () {
    return this._playerEvents
  }

  async update () {
    this._game = await Game.findById(this._game.id)
    return this
  }
}

GameHolder.getGameHolder = async gameId => {
  const game = await Game.findById(gameId)
  if (game) {
    return new GameHolder(game)
  }
}

module.exports = GameHolder
