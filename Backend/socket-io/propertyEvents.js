const SocketEmitter = require('./socketEmitter')

class PropertyEvents extends SocketEmitter {
  constructor (gameHolder) {
    super(gameHolder)

    this.onPropertyOwnerChanged = this.onPropertyOwnerChanged.bind(this)
    this.onPropertyMortgagedChanged = this.onPropertyMortgagedChanged.bind(this)
  }

  onPropertyOwnerChanged (propertyIndex, ownerId) {
    this.emit('propertyOwnerChanged', { propertyIndex, ownerId })
  }

  onPropertyMortgagedChanged (propertyIndex, mortgaged) {
    this.emit('propertyMortgagedChanged', { propertyIndex, mortgaged })
  }
}

module.exports = PropertyEvents
