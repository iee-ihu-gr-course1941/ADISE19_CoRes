const config = require('../config.json')

const express = require('express')
const { check, validationResult } = require('express-validator')

const Game = require('../models/game')
const User = require('../models/user')
const helpers = require('../helpers')

const router = express.Router()

module.exports = router

// Confirm user is logged in before continuing
router.use((req, res, next) => {
  if (req.session.user) {
    next()
  } else {
    return res.status(400).json({ error: { message: 'Not logged in' } })
  }
})

router.post('/new', [
  check('seats').isInt({ min: config.game.minPlayers, max: config.game.maxPlayers }),
  check('invite_only').optional().isBoolean() // TODO: Implement invites
], async (req, res, next) => {
  try {
    const errors = validationResult(req)
    if (!errors.isEmpty()) {
      return res.status(422).json({ errors: errors.array() })
    }

    // Check for active game
    if (req.session.game) {
      return res.status(400).json({ error: { message: 'Already in game' } })
    }

    // Initialize Game
    const game = new Game({
      players: [{
        user: req.session.user._id,
        balance: config.game.initialBalance,
        position: 0,
        duplicateRolls: 0,
        jailed: false
      }],
      seats: req.body.seats,
      status: 'waitingPlayers'
    })
    game.markModified('players')
    await game.save()

    // Update User in database
    const user = await User.findByIdAndUpdate(
      req.session.user._id,
      {
        $set: {
          lastGame: game.id,
          disconnected: 0
        }
      },
      {
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)
    req.session.game = game

    // TODO: Trigger game join event

    return res.json(game.toJSON())
  } catch (err) {
    next(err)
  }
})
