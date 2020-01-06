const { getGame } = require('../library/gameHolder')
const GameManager = require('../library/gameManager')

module.exports = async (req, res, next) => {
  // Try to load previous game
  const lastGameId = req.session.user && req.session.user.lastGame
  if (lastGameId) {
    const game = await getGame(lastGameId)

    // Check game.players for current user
    const players = game.getJSON().players
    const found = players.find(p => {
      const pUserId = p.user.toString()
      const sUserId = req.session.user._id
      return pUserId === sUserId
    })

    // Attach GameManager to res.locals
    if (found) {
      res.locals.game = new GameManager(req.session.user, game)
    }
  }

  // Attach empty GameManager if previous game is not attached
  if (!res.locals.game) {
    res.locals.game = new GameManager(req.session.user)
  }

  next()
}
