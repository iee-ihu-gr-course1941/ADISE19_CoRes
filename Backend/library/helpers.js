module.exports.transformUser = user => {
  const result = {
    ...user.toJSON(),
    _id: user.id
  }

  if (result.passwordHash) {
    delete result.passwordHash
  }

  return result
}
