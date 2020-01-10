/* global alert, fetch */

import React from 'react'
import ReactDOM from 'react-dom'
import io from 'socket.io-client'

import Game from './components/Game'
import Lobby from './components/Lobby'
import Login from './components/Login'

window.onload = async () => {
  const socket = io()
  socket.on('playerJoined', data => {
    alert('Joined ' + data.player.user)
  })

  const root = document.getElementById('root')
  const logout = document.getElementById('logout')

  let session

  session = await fetch('/auth/session')
    .then(r => r.status === 200 || r.status === 304 ? r.json() : null)

  const handleLogout = () => {
    fetch('/auth/logout')
      .then(r => {
        if (r.status === 200 || r.status === 304) {
          session = null
          init()
        }
      })
  }

  const handleLogin = data => {
    session = data
    init()
  }

  const handleJoin = game => {
    ReactDOM.render(<Game game={game} onLeave={handleLeave} />, root)
  }

  const handleLeave = () => {
    fetch('/game/leave')
      .then(async r => {
        const response = await r.json()
        if (r.status === 200 || r.status === 304) {
          init()
        } else {
          console.error(response)
        }
      })
  }

  const init = () => {
    if (session) {
      ReactDOM.render((
        <input type='button' value='Logout' onClick={handleLogout} />
      ), logout)

      fetch('/game/current')
        .then(async r => {
          const response = await r.json()
          if (r.status === 200 || r.status === 304) {
            handleJoin(response)
          } else {
            ReactDOM.render(<Lobby onJoin={handleJoin} />, root)
          }
        })
    } else {
      ReactDOM.render((
        <Login onLogin={handleLogin} />
      ), root)
      ReactDOM.render(null, logout)
    }
  }

  init()
}
