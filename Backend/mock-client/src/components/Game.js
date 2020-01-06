import React from 'react'

export default function (props) {
  return (
    <div>
      <input type='button' value='Leave Game' onClick={props.onLeave} />
      <pre style={{ fontFamily: 'monospace' }}>
        {JSON.stringify(props.game, null, 2)}
      </pre>
    </div>
  )
}
