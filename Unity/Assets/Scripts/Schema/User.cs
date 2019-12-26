using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Schema
{
    public class User
    {
        #region Caching

        private static Dictionary<string, User> _users;

        public static User GetUser(JToken user)
        {
            if (_users == null)
            {
                _users = new Dictionary<string, User>();
            }
            
            string userId = (string) user["_id"];
            User dUser = _users[userId];

            if (dUser == null)
            {
                User newUser = new User(user);
                _users.Add(userId, newUser);
                dUser = newUser;
            }

            return dUser;
        }

        #endregion

        private string _id;
        private string _username;
        private string _lastGame;
        private DateTime _disconnected;

        public string Id => _id;
        public string Username => _username;
        public string LastGame => _lastGame;
        public DateTime Disconnected => _disconnected;

        private User(JToken user)
        {
            _id = (string) user["_id"];
            _username = (string) user["username"];
            _lastGame = (string) user["lastGame"];
            _disconnected = DateTime.Parse((string) user["disconnected"]);
        }
    }
}
