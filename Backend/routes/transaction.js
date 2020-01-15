const config = require('../config.json')

const express = require('express')

const Game = require('../models/game')

const GameError = require('../library/gameError')

const router = express.Router()

module.exports = router

// Confirm user is logged in and is playing a game before continuing
router.use((req, res, next) => {
  if (!req.session.user) {
    return res.status(400).json({ error: { message: 'Not logged in' } })
  }

  if (!res.locals.game.current()) {
    return res.status(400).json({ error: { message: 'Not in game' } })
  }

  if (res.locals.game.current().currentPlayer.toString() !== req.session.user._id) {
    return res.status(400).json({ error: { message: 'Not your turn' } })
  }

  next()
})

router.get('/buy-current-property', async (req, res, next) => {
  try {
    const currentGame = res.locals.game.current()

    const player = currentGame.players.find(p => p.user.toString() === req.session.user._id)

    const configProperties = config.prices.properties

    const currentPropertyIndex = configProperties.findIndex(p => p.location === player.position)
    if (currentPropertyIndex < 0) {
      throw new GameError('You are not standing on any property')
    }

    const currentProperty = currentGame.properties[currentPropertyIndex]

    if (currentProperty.owner) {
      if (currentProperty.owner.toString() === req.session.user._id) {
        throw new GameError('You already own this property')
      } else {
        throw new GameError('This property is owned by another player')
      }
    }

    const propertyPrice = configProperties[currentPropertyIndex].price

    if (propertyPrice > player.balance) {
      throw new GameError('You do not have enough money')
    }

    const game = await Game.findById(currentGame._id)
    const gamePlayer = game.players.find(p => p.user.toString() === req.session.user._id)
    gamePlayer.balance -= propertyPrice
    game.properties[currentPropertyIndex].owner = req.session.user._id

    const gameHolder = res.locals.game.getGameHolder()
    gameHolder.getPlayerEvents().onPlayerPaid(player.user, propertyPrice)
    gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
    gameHolder.getPropertyEvents().onPropertyOwnerChanged(currentPropertyIndex, req.session.user._id)

    await game.save()
    await gameHolder.update()

    return res.json(game.properties[currentPropertyIndex])
  } catch (err) {
    next(err)
  }
})
