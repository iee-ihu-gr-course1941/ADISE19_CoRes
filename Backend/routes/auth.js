const bcrypt = require('bcrypt')
const express = require('express')
const { check, validationResult } = require('express-validator')

const SocketManager = require('../socket-io/socketManager')

const User = require('../models/user')
const helpers = require('../library/helpers')

const router = express.Router()

module.exports = router

router.get('/session', (req, res) => {
  SocketManager.updateSocketsUserFromSession(req.sessionID,
    req.session.user && req.session.user._id)
  
  if (req.session.user) {
    return res.json({
      user: req.session.user,
      in_game: Boolean(res.locals.game.current()),
      was_disconnected: !req.session.user.leftGame // True: Player logged out or lost connection during last game
    })
  }

  return res.status(403).json({ error: { message: 'Not logged in' } })
})

router.post('/login', [
  check('username').isLength({ min: 4 }),
  check('password').isLength({ min: 6 })
], async (req, res, next) => {
  try {
    const errors = validationResult(req)
    if (!errors.isEmpty()) {
      return res.status(422).json({ errors: errors.array() })
    }

    if (req.session.user) {
      return res.status(400).json({ error: { message: 'Already logged in' } })
    }

    const user = await User.findOne({ username: req.body.username })
    if (user) {
      const match = bcrypt.compareSync(req.body.password, user.passwordHash) // TODO: Make async

      // Delete password from request body
      delete req.body.password

      // Delete passwordHash from User object
      delete user.passwordHash
      delete user._doc.passwordHash

      if (match) {
        req.session.user = helpers.transformUser(user)
        return res.redirect('./session')
      }
    }

    return res.status(401).json({ error: { message: 'Invalid Credentials' } })
  } catch (err) {
    next(err)
  }
})

router.get('/logout', async (req, res, next) => {
  try {
    if (req.session.user) {
      if (res.locals.game.current()) {
        // TODO: Handle game disconnect
        await User.findByIdAndUpdate(
          req.session.user._id,
          {
            $set: { disconnected: Date.now() }
          },
          {
            runValidators: true,
            select: '-passwordHash'
          }
        )
      }

      delete req.session.user
      SocketManager.updateSocketsUserFromSession(req.sessionID)
      return res.json({ message: 'Logged out successfully' })
    }

    return res.status(403).json({ error: { message: 'Not logged in' } })
  } catch (err) {
    next(err)
  }
})

router.post('/register', [
  check('username').isLength({ min: 4 }),
  check('password').isLength({ min: 6 })
], async (req, res, next) => {
  try {
    const errors = validationResult(req)
    if (!errors.isEmpty()) {
      return res.status(422).json({ errors: errors.array() })
    }

    if (req.session.user) {
      return res.status(400).json({ error: { message: 'Already logged in' } })
    }

    const userCount = await User.count({ username: req.body.username })
    if (userCount > 0) {
      return res.status(400).json({ error: { message: 'Username already exists' } })
    }

    const hash = bcrypt.hashSync(req.body.password, 10) // TODO: Make async
    delete req.body.password

    const user = new User({
      username: req.body.username,
      passwordHash: hash
    })
    await user.save()

    // Delete passwordHash from User object
    delete user.passwordHash
    delete user._doc.passwordHash

    req.session.user = helpers.transformUser(user)

    return res.redirect('./session')
  } catch (err) {
    next(err)
  }
})
