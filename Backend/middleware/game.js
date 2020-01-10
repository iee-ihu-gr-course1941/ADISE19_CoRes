const GameManager = require('../library/gameManager')

module.exports = async (req, res, next) => {
  try {
    // Attach GameManager to res.locals
    const gameManager = new GameManager(req.session.user)
    await gameManager.init()
    res.locals.game = gameManager

    next()
  } catch (err) {
    next(err)
  }
}
