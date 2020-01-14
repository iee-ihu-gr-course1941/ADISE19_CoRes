using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Schema
{
    public class Player
    {
        #region Caching

        private static Dictionary<string, Player> _players;

        public static Player GetPlayer(JToken player)
        {
            if (_players == null)
            {
                _players = new Dictionary<string, Player>();
            }

            string userId = (string) player["user"];

            if (!_players.ContainsKey(userId))
            {
                Player newPlayer = new Player(player);
                _players.Add(userId, newPlayer);
            }

            return _players[userId];
        }
        
        public static Player GetPlayerById(string userId)
        {
            return _players[userId];
        }

        #endregion
        
        private string _userId;
        private int _balance;
        private int _position;
        private int _duplicateRolls;
        private bool _jailed;
        private int _index;
        private string _name;

        public string UserId => _userId;
        public int Balance => _balance;
        public int Position => _position;
        public int DuplicateRolls => _duplicateRolls;
        public bool Jailed => _jailed;
        public int Index => _index;
        public string Name => _name;

        private Player(JToken player)
        {
            _userId = (string) player["user"];
            _balance = (int) player["balance"];
            _position = (int) player["position"];
            _duplicateRolls = (int) player["duplicateRolls"];
            _jailed = (bool) player["jailed"];
            _index = (int) player["index"];
            _name = (string) player["name"];
        }

        public void SetPosition(int position)
        {
            _position = position;
        }
        
        public static void ClearCache()
        {
            _players?.Clear();
        }
    }
}
