using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Schema;

public class Session
{
   private static Session _session;
   public static Session Instance => _session;
   
   private User _user;
   private bool _inGame;
   private bool _wasDisconnected;

   public User User => _user;
   public bool InGame => _inGame;
   public bool WasDisconnected => _wasDisconnected;

   public Session(JToken session)
   {
      _user = User.GetUser(session["user"]);
      _inGame = (bool) session["in_game"];
      _wasDisconnected = (bool) session["was_disconnected"];

      _session = this;
   }
}
