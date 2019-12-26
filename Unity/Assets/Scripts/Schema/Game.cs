using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Schema
{
    public class Game
    {
        #region Caching

        private static Dictionary<string, Game> _games;

        public static Game GetGame(JToken game)
        {
            if (_games == null)
            {
                _games = new Dictionary<string, Game>();
            }
            
            string gameId = (string) game["_id"];
            Game dGame = _games[gameId];

            if (dGame == null)
            {
                Game newGame = new Game(game);
                _games.Add(gameId, newGame);
                dGame = newGame;
            }

            return dGame;
        }

        #endregion

        private string _id;
        private List<Player> _players;
        private List<Property> _properties;
        private int _seats;
        private string _status;
        private User _currentPlayer;

        public string Id => _id;
        public List<Player> Players => _players;
        public List<Property> Properties => _properties;
        public int Seats => _seats;
        public string Status => _status;
        public User CurrentPlayer => _currentPlayer;

        private Game(JToken game)
        {
            _id = (string) game["_id"];
            _players = ((JArray) game["players"]).Select(Player.GetPlayer).ToList();
            _properties = ((JArray) game["properties"]).Select(Property.GetProperty).ToList();
            _seats = (int) game["seats"];
            _status = (string) game["status"];
            _currentPlayer = User.GetUser(game["currentPlayer"]);
        }
    }
}
