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

            if (!_games.ContainsKey(gameId))
            {
                Game newGame = new Game(game);
                _games.Add(gameId, newGame);
            }
            
            return _games[gameId];
        }

        #endregion

        private string _id;
        private List<Player> _players;
        private List<Property> _properties;
        private int _seats;
        private string _status;
        private string _currentPlayerId;

        public string Id => _id;
        public List<Player> Players => _players;
        public List<Property> Properties => _properties;
        public int Seats => _seats;
        public string Status => _status;
        public string CurrentPlayerId => _currentPlayerId;

        private Game(JToken game)
        {
            _id = (string) game["_id"];
            _players = ((JArray) game["players"]).Select(Player.GetPlayer).ToList();
            _properties = ((JArray) game["properties"]).Select(Property.GetProperty).ToList();
            _seats = (int) game["seats"];
            _status = (string) game["status"];
            _currentPlayerId = (string) game["currentPlayer"];
        }

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public void SetRunning()
        {
            _status = "running";
        }

        public static void ClearCache()
        {
            _games?.Clear();
        }

        public string SeatsToString() => $"{_players.Count}/{_seats}";
    }
}
