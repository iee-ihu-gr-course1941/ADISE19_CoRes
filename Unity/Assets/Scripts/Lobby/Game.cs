using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Game
{
    private JToken _game;

    public Game(JToken game)
    {
        _game = game;
    }

    public override string ToString()
    {
        int seats = (int) _game["seats"];
        JArray players = (JArray) _game["players"];

        return $"{players.Count}/{seats}";
    }
}
