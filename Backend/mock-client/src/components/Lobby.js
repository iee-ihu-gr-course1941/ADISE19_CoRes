/* global fetch */

import React, { useState, useEffect } from 'react'

export default function (props) {
  const [games, setGames] = useState([])
  const [values, setValues] = useState({
    seats: 0
  })

  const handleChange = e => {
    setValues({ ...values, [e.target.name]: e.target.value })
  }

  const handleCreate = () => {
    fetch('/game/new', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ seats: values.seats })
    })
      .then(async r => {
        const response = await r.json()
        if (r.status === 200 || r.status === 304) {
          props.onJoin(response)
        } else {
          console.error(response)
        }
      })
  }

  const handleJoin = id => () => {
    fetch('/game/join', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ game_id: id })
    })
      .then(async r => {
        const response = await r.json()
        if (r.status === 200 || r.status === 304) {
          props.onJoin(response)
        } else {
          console.error(response)
        }
      })
  }

  useEffect(() => {
    fetch('/game/list')
      .then(async r => {
        const response = await r.json()
        if (r.status === 200 || r.status === 304) {
          setGames(response)
        } else {
          console.error(response)
        }
      })
  }, [])

  return (
    <div>
      <div>
        <input type='number' min='2' max='4' placeholder='Seats' name='seats' value={values.seats} onChange={handleChange} />
        <input type='button' value='Create Game' onClick={handleCreate} />
      </div>
      <ul style={{ fontFamily: 'monospace' }}>
        {games.map(g => g.status === 'waitingPlayers' && (
          <li key={g._id}>
            id: {g._id}
            <br />
            players: {g.players.length}/{g.seats}
            <br />
            <input type='button' value='Join' onClick={handleJoin(g._id)} />
          </li>
        ))}
      </ul>
    </div>
  )
}
