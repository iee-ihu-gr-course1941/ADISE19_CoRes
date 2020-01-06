const Game = require('../models/game')

const GameError = require('./gameError')

// Cache games
const games = {}

class GameHolder {
  constructor (game) {
    if (!game) {
      throw new GameError('game cannot be empty')
    }

    this._game = game
    // TODO: Subscribe to change stream

    this.getJSON = this.getJSON.bind(this)
  }

  getJSON () {
    return this._game.toJSON()
  }
}

const getGame = async (gameId, options = { update: false }) => {
  if (!games[gameId] || options.update) {
    const game = await Game.findById(gameId)
    games[gameId] = new GameHolder(game)
  }

  return games[gameId]
}

module.exports = { GameHolder, getGame }
