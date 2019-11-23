const bcrypt = require('bcrypt')
const express = require('express')
const { check, validationResult } = require('express-validator')

const User = require('../models/user')

const router = express.Router()

module.exports = router

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
      const match = bcrypt.compareSync(req.body.password, user.passwordHash)
      delete req.body.password
      if (match) {
        req.session.user = {
          _id: user.id,
          username: user.username
        }
        return res.json({ message: 'Logged in successfully' })
      }
    }

    return res.status(401).json({ error: { message: 'Invalid Credentials' } })
  } catch (err) {
    next(err)
  }
})

router.get('/logout', (req, res) => {
  if (req.session.user) {
    delete req.session.user
    return res.json({ message: 'Logged out successfully' })
  }

  return res.status(403).json({ error: { message: 'Not logged in' } })
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

    const user = await User.findOne({ username: req.body.username })
    if (!user) {
      const hash = bcrypt.hashSync(req.body.password, 10)
      delete req.body.password

      const user = new User({
        username: req.body.username,
        passwordHash: hash
      })
      await user.save()

      req.session.user = { username: user.username }

      return res.json({ message: 'User registered successfully' })
    }

    return res.status(400).json({ error: { message: 'Username already exists' } })
  } catch (err) {
    next(err)
  }
})
