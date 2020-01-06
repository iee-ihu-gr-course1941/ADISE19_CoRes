/* global fetch */

import React, { useState } from 'react'

export default function (props) {
  const [values, setValues] = useState({
    loginUsername: '',
    loginPassword: '',
    registerUsername: '',
    registerPassword: ''
  })

  const handleChange = e => {
    setValues({ ...values, [e.target.name]: e.target.value })
  }

  const handleLogin = () => {
    fetch('/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username: values.loginUsername,
        password: values.loginPassword
      })
    })
      .then(async r => {
        const response = await r.json()
        if (r.status === 200 || r.status === 304) {
          props.onLogin(response)
        } else {
          console.error(response)
        }
      })
  }

  const handleRegister = () => {
    fetch('/auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        username: values.registerUsername,
        password: values.registerPassword
      })
    })
      .then(async r => {
        const response = await r.json()
        if (r.status === 200 || r.status === 304) {
          props.onLogin(response)
        } else {
          console.error(response)
        }
      })
  }

  return (
    <div>
      <table>
        <tbody>
          <tr>
            <td>
              <form>
                <h3>Login</h3>
                <input type='text' placeholder='Username' name='loginUsername' value={values.loginUsername} onChange={handleChange} />
                <br />
                <input type='text' placeholder='Password' name='loginPassword' value={values.loginPassword} onChange={handleChange} />
                <br />
                <input type='button' value='Submit' onClick={handleLogin} />
              </form>
            </td>
            <td>
              <form>
                <h3>Register</h3>
                <input type='text' placeholder='Username' name='registerUsername' value={values.registerUsername} onChange={handleChange} />
                <br />
                <input type='text' placeholder='Password' name='registerPassword' value={values.registerPassword} onChange={handleChange} />
                <br />
                <input type='button' value='Submit' onClick={handleRegister} />
              </form>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  )
}
