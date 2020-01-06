class GameError extends Error {
  constructor (error) {
    super(error)
    this._isGameError = true
  }

  toJSON () {
    return {
      message: this.message
    }
  }
}

module.exports = GameError
