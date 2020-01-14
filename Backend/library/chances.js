const Game = require('../models/game')

module.exports = [
  {
    text: 'Πήγαινε στο Τμήμα Ανατολικών Γλωσσών. Αν περάσεις την αφετηρία πάρε 200ΔΜ.',
    action: async (userId, gameHolder) => {
      const targetPropertyLocation = 24

      const currentGame = gameHolder.getJSON()
      const player = currentGame.players.find(p => p.user.toString() === userId.toString())

      const game = await Game.findById(currentGame._id)
      const gamePlayer = game.players.find(p => p.user.toString() === userId.toString())

      if (player.position > targetPropertyLocation) {
        gamePlayer.balance += 200

        gameHolder.getPlayerEvents().onPlayerPassedFromGo(player.user)
        gameHolder.getPlayerEvents().onPlayerGotPaid(player.user, 200)
        gameHolder.getPlayerEvents().onPlayerBalanceChanged(player.user, gamePlayer.balance)
      }
      gamePlayer.position = targetPropertyLocation
      await game.save()
      await gameHolder.update()
    }
  }
]
