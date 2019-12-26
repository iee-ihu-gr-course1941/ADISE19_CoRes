using System;
using System.Collections;
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

            string userId = User.GetUser(player["user"]).Id;
            Player dPlayer = _players[userId];

            if (dPlayer == null)
            {
                Player newPlayer = new Player(player);
                _players.Add(userId, newPlayer);
                dPlayer = newPlayer;
            }

            return dPlayer;
        }

        #endregion
        
        private User _user;
        private int _balance;
        private int _position;
        private int _duplicateRolls;
        private bool _jailed;

        public User User => _user;
        public int Balance => _balance;
        public int Position => _position;
        public int DuplicateRolls => _duplicateRolls;
        public bool Jailed => _jailed;

        private Player(JToken player)
        {
            _user = User.GetUser(player["user"]);
            _balance = (int) player["balance"];
            _position = (int) player["position"];
            _duplicateRolls = (int) player["duplicateRolls"];
            _jailed = (bool) player["jailed"];
        }
    }
}
