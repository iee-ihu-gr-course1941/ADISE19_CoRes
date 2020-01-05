using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Schema;

public class GameManager : MonoBehaviour
{
    #region Singleton
    
    public static GameManager Instance { get => _instance; }

    private static GameManager _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    #endregion
    
    private Game _game;

    public enum Turn
    {
        PLAYER1, PLAYER2, PLAYER3, PLAYER4
    }
    public Turn currentPlayerTurn = Turn.PLAYER1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextTurn();
        }
    }

    public void GoToGame(Game game)
    {
        _game = game;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    
    public void NextTurn()
    {
        var length = System.Enum.GetNames(typeof(Turn)).Length;
        currentPlayerTurn = (Turn)(
            ((int)currentPlayerTurn + 1) % length
        );
    }
}
