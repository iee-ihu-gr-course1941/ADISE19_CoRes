const http = require('http')

const bodyParser = require('body-parser')
const express = require('express')
const expressSession = require('express-session')
const mongoose = require('mongoose')
const sharedSession = require('express-socket.io-session')
const socketIo = require('socket.io')

const authRouter = require('./routes/auth')

const app = express()
const server = http.Server(app)
const io = socketIo(server)

const session = expressSession({
  secret: process.env.COOKIE_SECRET || 'key',
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false }
})

io.use(sharedSession(session, { autoSave: true }))

io.on('connection', socket => {
  socket.emit('hi', { message: 'Success' })
})

app.use(bodyParser.json())
app.use(session)

app.use('/auth', authRouter)

server.listen(3000, err => {
  if (err) throw err
  console.log('Listening...')
})

app.use(function (err, req, res, next) {
  console.error(err.stack)
  res.status(500).send('Internal Server Error')
})
