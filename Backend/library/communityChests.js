module.exports = [
  {
    text: "Πήγαινε στην Αφετηρία.",
    action: async (userId, gameHolder) => {
      const targetLocation = 0

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 200

      gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      gamePlayer.position = targetLocation
      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: "Έκανες την απαλλακτική εργασία σου. Πάρε 100ΔΜ.",
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance += 100
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 100)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  },
  {
    text: "Έμεινες στο εργαστηριακό μέρος μαθήματος. Χάνεις 50ΔΜ.",
    action: async (userId, gameHolder) => {
      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      gamePlayer.balance -= 50
      gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 50)
      gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)

      await game.save()
      await gameHolder.update()
    }
  }
]
