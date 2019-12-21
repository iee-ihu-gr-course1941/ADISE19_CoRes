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
    let game = new Game({
      players: [{
        user: req.session.user._id,
        balance: config.game.initialBalance,
        position: 0,
        duplicateRolls: 0,
        jailed: false
      }],
      seats: req.body.seats,
      status: 'waitingPlayers',
      properties: config.prices.properties
    })
    game = await game.save()

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
        new: true,
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)
    req.session.game = game.toJSON()

    // TODO: Trigger game join event

    return res.json(game.toJSON())
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
    if (req.session.game) {
      return res.status(400).json({ error: { message: 'Already in game' } })
    }

    // Find game in DB
    const game = await Game.findById(req.body.game_id)

    // Check if game was found
    if (!game) {
      return res.status(404).json({ error: { message: 'Game not found' } })
    }

    // Check if user is in game.players
    const playerIndex = game.players.findIndex(p => p.user.toString() === req.session.user._id)
    if (playerIndex >= 0) { // Reconnect if user was left in that game
      if (game.status === 'ended') {
        return res.status(400).json({ error: { message: 'Game has ended' } })
      }

      if (game.status === 'running') {
        const disconnectTime = new Date(req.session.user.disconnected).getTime()
        if (disconnectTime + config.game.msToReconnect < Date.now()) {
          // TODO: Player should have automatically been kicked from game by this time
          return res.status(400).json({ error: { message: 'You cannot reconnect to this game' } })
        }
      }
    } else { // Try to join game
      // Check for empty seats
      if (game.players.length > game.seats) {
        // TODO: Unexpected behavior
        return res.status(501).json({ error: { message: 'Game has more players than expected' } })
      } else if (game.players.length === game.seats) {
        return res.status(400).json({ error: { message: 'Game is full' } })
      }

      // Check game status
      if (game.status !== 'waitingPlayers') {
        return res.status(400).json({ error: { message: 'This game is not waiting for players to join' } })
      }

      // Add user to game.players
      game.players.push({
        user: req.session.user._id,
        balance: config.game.initialBalance,
        position: 0,
        duplicateRolls: 0,
        jailed: false
      })
      await game.save()
    }

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
        new: true,
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)
    req.session.game = game.toJSON()

    // TODO: Trigger game join event

    return res.json(game.toJSON())
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

router.get('/leave', async (req, res, next) => {
  try {
    const errors = validationResult(req)
    if (!errors.isEmpty()) {
      return res.status(422).json({ errors: errors.array() })
    }

    // Check for active game
    if (!req.session.game) {
      return res.status(400).json({ error: { message: 'User is not playing any games' } })
    }

    // Find game in DB
    const game = await Game.findById(req.session.game._id)

    // Check if game was found
    if (!game) {
      // TODO: Unexpected behavior
      return res.status(501).json({ error: { message: 'Game not found' } })
    }

    // Check if user is in game.players
    const playerIndex = game.players.findIndex(p => p.user.toString() === req.session.user._id)
    if (playerIndex < 0) {
      // TODO: Unexpected behavior
      return res.status(501).json({ error: { message: 'User is not playing in this game' } })
    }

    // Remove user from game.players
    game.players.splice(playerIndex, 1)
    await game.save()

    // TODO: Trigger game leave event

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
        new: true,
        runValidators: true,
        select: '-passwordHash'
      }
    )

    // Update session
    req.session.user = helpers.transformUser(user)
    delete req.session.game

    return res.json({ message: 'Left game successfully' })
  } catch (err) {
    next(err)
  }
})
