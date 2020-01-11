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

    public Game Game => _game;

    public void GoToGame(Game game)
    {
        _game = game;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    
    public void GoToLobby(Game game)
    {
        _game = game;
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }
}
