const http = require('http')

require('dotenv').config()

const bodyParser = require('body-parser')
const express = require('express')
const expressSession = require('express-session')
const logger = require('morgan')
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

app.use(logger('short'))
app.use(bodyParser.json())
app.use(bodyParser.urlencoded({ extended: false }))
app.use(session)

app.use('/auth', authRouter)

mongoose.set('useCreateIndex', true)
mongoose.set('useUnifiedTopology', true)
mongoose
  .connect(
    process.env.MONGODB_CLUSTER
      ? `mongodb+srv://${process.env.MONGODB_USER}:${process.env.MONGODB_PASS}@${
          process.env.MONGODB_CLUSTER
        }`
      : 'mongodb://localhost/monopoliee',
    { useNewUrlParser: true }
  )
  .then(() => {
    const port = process.env.PORT || 3000
    server.listen(port, err => {
      if (err) throw err
      console.log(`Listening on port ${port}...`)
    })
  })
  .catch(err => {
    console.error(err)
  })

app.use(function (err, req, res, next) {
  console.error(err.stack)
  res.status(500).send({ error: { message: 'Internal Server Error' } })
})
