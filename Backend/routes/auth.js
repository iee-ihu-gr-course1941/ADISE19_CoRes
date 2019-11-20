const express = require('express')
const { check, validationResult } = require('express-validator')

const router = express.Router()

module.exports = router

router.post('/login', [
  check('username').isLength({ min: 4 }),
  check('password').isLength({ min: 6 })
], (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(422).json({ errors: errors.array() });
  }
})

router.post('/register', [
  check('username').isLength({ min: 4 }),
  check('password').isLength({ min: 6 })
], (req, res) => {
  const errors = validationResult(req);
  if (!errors.isEmpty()) {
    return res.status(422).json({ errors: errors.array() });
  }
})
