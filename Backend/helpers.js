module.exports.transformUser = user => {
  const result = {
    ...user._doc,
    _id: user.id
  }

  if (result.passwordHash) {
    delete result.passwordHash
  }

  return result
}
