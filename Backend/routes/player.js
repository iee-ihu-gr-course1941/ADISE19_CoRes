const express = require('express')
const { Types } = require('mongoose')

const Game = require('../models/game')

const GameError = require('../library/gameError')

const router = express.Router()

module.exports = router

const nextPlayerTurn = async gameHolder => {
  const game = gameHolder.getJSON()

  const players = game.players.slice().sort((a, b) => a.index - b.index)

  const currentPlayerIndex = players.find(p => p.user.toString() === game.currentPlayer.toString()).index
  let nextPlayer = players.find(p => p.index > currentPlayerIndex)

  if (!nextPlayer) {
    nextPlayer = players[0]
  }

  const updatedGame = await Game.updateOne(
    { _id: Types.ObjectId(game._id) },
    {
      $set: {
        currentPlayer: Types.ObjectId(nextPlayer.user)
      }
    },
    {
      new: true,
      runValidators: true
    }
  ).then(() => gameHolder.update())

  gameHolder.getPlayerEvents().onPlayerTurnChanged(nextPlayer.user)

  return updatedGame
}

// Confirm user is logged in and is playing a game before continuing
router.use((req, res, next) => {
  if (!req.session.user) {
    return res.status(400).json({ error: { message: 'Not logged in' } })
  }

  if (!res.locals.game.current()) {
    return res.status(400).json({ error: { message: 'Not in game' } })
  }

  next()
})

router.get('/roll-dice', async (req, res, next) => {
  try {
    const currentGame = res.locals.game.current()

    if (currentGame.currentPlayer.toString() !== req.session.user._id) {
      throw new GameError('Not your turn')
    }

    if (req.session.rolled) {
      throw new GameError('You have already rolled the dice')
    }

    const dice = [
      Math.floor(Math.random() * 6),
      Math.floor(Math.random() * 6)
    ]

    const player = currentGame.players.find(p => p.user.toString() === req.session.user._id)

    const diceSum = dice[0] + dice[1]
    const newLocation = (player.position + diceSum) % 40

    const gameHolder = res.locals.game.getGameHolder()
    gameHolder.getPlayerEvents().onPlayerRolledDice(player.user, dice)

    const game = await Game.findById(currentGame._id)
    game.players.find(p => p.user.toString() === req.session.user._id).position = newLocation
    await game.save()
    await gameHolder.update()

    gameHolder.getPlayerEvents().onPlayerMoved(player.user, newLocation)

    req.session.dice = dice
    req.session.rolled = true

    return res.json({ dice })
  } catch (err) {
    next(err)
  }
})

router.get('/end-turn', (req, res, next) => {
  try {
    const game = res.locals.game.current()

    if (game.currentPlayer.toString() !== req.session.user._id) {
      throw new GameError('Not your turn')
    }

    if (!req.session.dice) {
      throw new GameError('You need to roll the dice')
    }

    const player = game.players.find(p => p.user.toString() === req.session.user._id)

    const gameHolder = res.locals.game.getGameHolder()

    let nextPlayer

    const dice = req.session.dice
    if (dice[0] === dice[1]) {
      gameHolder.getPlayerEvents().onPlayerPlaysAgain(req.session.user._id)
      nextPlayer = player
    } else {
      nextPlayer = nextPlayerTurn(gameHolder)
    }
    delete req.session.dice
    delete req.session.rolled

    return res.json({ nextPlayer })
  } catch (err) {
    next(err)
  }
})
