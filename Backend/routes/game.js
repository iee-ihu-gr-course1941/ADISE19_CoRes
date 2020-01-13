const config = require('../config.json')

const express = require('express')
const { check, validationResult } = require('express-validator')

const Game = require('../models/game')
const User = require('../models/user')
const helpers = require('../library/helpers')

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
    if (res.locals.game.current()) {
      return res.status(400).json({ error: { message: 'Already in game' } })
    }

    const game = await res.locals.game.create(req.body.seats)

    // Update User in database
    const user = await User.findByIdAndUpdate(
      req.session.user._id,
      {
        $set: {
          lastGame: game._id,
          disconnected: 0
        }
      },
      {
        new: true,
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)

    return res.json(game)
  } catch (err) {
    next(err)
  }
})

router.post('/join', [
  check('game_id').isMongoId(),
  check('invitation_code').optional().isMongoId() // TODO: Implement invites
], async (req, res, next) => {
  try {
    const errors = validationResult(req)
    if (!errors.isEmpty()) {
      return res.status(422).json({ errors: errors.array() })
    }

    // Check for active game
    if (res.locals.game.current()) {
      const game = res.locals.game.current()
      if (game._id === req.body.game_id) {
        // TODO: Trigger player reconnect
        return res.json(game)
      }
      return res.status(400).json({ error: { message: 'Already in game' } })
    }

    const game = await res.locals.game.join(req.body.game_id)

    // Update User in database
    const user = await User.findByIdAndUpdate(
      req.session.user._id,
      {
        $set: {
          lastGame: game._id,
          disconnected: 0
        }
      },
      {
        new: true,
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)

    return res.json(game)
  } catch (err) {
    next(err)
  }
})

router.get('/list', async (req, res, next) => {
  try {
    const games = await Game.find({
      status: { $ne: 'ended' }
    }).lean().exec()

    res.json(games)
  } catch (err) {
    next(err)
  }
})

router.get('/current', async (req, res, next) => {
  try {
    // Check for active game
    const game = res.locals.game.current()
    if (!game) {
      return res.status(400).json({ error: { message: 'User is not playing any games' } })
    }

    res.json(game)
  } catch (err) {
    next(err)
  }
})

router.get('/leave', async (req, res, next) => {
  try {
    const errors = validationResult(req)
    if (!errors.isEmpty()) {
      return res.status(422).json({ errors: errors.array() })
    }

    const game = await res.locals.game.leave()

    // Update User in database
    const user = await User.findByIdAndUpdate(
      req.session.user._id,
      {
        $set: {
          lastGame: game._id,
          disconnected: 0
        }
      },
      {
        new: true,
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)

    return res.json({ message: 'Left game successfully' })
  } catch (err) {
    next(err)
  }
})

router.get('/prices', (req, res, next) => {
  try {
    res.json(config.prices)
  } catch (err) {
    next(err)
  }
})
