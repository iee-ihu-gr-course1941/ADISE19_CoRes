using System;
using System.Collections.Generic;
using System.Linq;
using Schema;
using TMPro;
using UnityEngine;

public class CurrentGame : MonoBehaviour
{
    private bool diceRolled = false;
    private int playerNextLocation = -1;
    public SocketIo socketIo;
    public GameObject bottomBar;
    public GameObject[] playerPrefabs = new GameObject[4];
    public GameObject GoNode;
    public GameObject players;
    public GameObject nowPlayingPlayer;
    public CameraController CameraController;
    public Dice diceContainer;

    private readonly Vector3[] _offsets = {
        new Vector3(0.15f, 0, 0.15f),
        new Vector3(-0.15f, 0, 0.15f), 
        new Vector3(0.15f, 0, -0.15f),
        new Vector3(-0.15f, 0, -0.15f)
    };
    
    public List<GameObject> playerList;

    private void Start()
    {
        CameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;
        socketIo.PlayerRolledDice += SocketIoOnPlayerRolledDice;
        socketIo.PlayerMoved+= SocketIoOnPlayerMoved;
        socketIo.PlayerTurnChanged += SocketIoOnPlayerTurnChanged;

        UpdateBottomBar();
        SetupPlayers();
        // playerList.Reverse();
        
        foreach (Transform child in players.transform)
        {
            if (child != players.transform)
            {
                playerList.Add(child.gameObject);
            }
        }

        var player = Player.GetPlayerById(GameManager.Instance.Game.CurrentPlayerId);
        UpdateBottomBarPlayerPlaying(player);
        
        CameraController.SetUpCameras();
    }

    private void Update()
    {
        if (diceRolled && playerNextLocation > -1)
        {
            var player = Player.GetPlayerById(GameManager.Instance.Game.CurrentPlayerId);
            var playerObject = playerList[player.Index];
            
            StartCoroutine(playerObject.GetComponent<PlayerMovement>().Move(playerNextLocation));
            
            diceRolled = false;
            playerNextLocation = -1;
        }
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        UpdateBottomBar();
        AddPlayer(player);
    }
    
    private void SocketIoOnPlayerLeft(Player player)
    {
        UpdateBottomBar();
    }
    
    private void SocketIoOnPlayerRolledDice(Player player, int[] dice)
    {
        diceRolled = true;
        StartCoroutine(diceContainer.RollTheDice(dice));
    }
    
    private void SocketIoOnPlayerMoved(Player player, int location)
    {
        GameManager.Instance.Game.UpdateCurrentPlayer(player);
        playerNextLocation = location;
    }

    private void SocketIoOnPlayerTurnChanged(Player player)
    {
        UpdateBottomBarPlayerPlaying(player);
        CameraController.FocusCameraOn(player);
    }

    private void SetupPlayers()
    {
        var game = GameManager.Instance.Game;

        foreach (var player in game.Players)
        {
            AddPlayer(player);
        }
    }

    private void AddPlayer(Player player)
    {
        var playerPos = GoNode.transform.position + _offsets[player.Index];
        var newPlayer = Instantiate(playerPrefabs[player.Index], playerPos, Quaternion.identity, players.transform);
        newPlayer.GetComponent<PlayerMovement>().offset = _offsets[player.Index];
    }

    private void UpdateBottomBarPlayerPlaying(Player player)
    {
        var currentPlayerIndex = player.Index;

        switch (currentPlayerIndex)
        {
            case 0:
                nowPlayingPlayer.transform.localPosition = new Vector3(-720, 0 , 0);
                break;
            case 1:
                nowPlayingPlayer.transform.localPosition = new Vector3(-240, 0 , 0);
                break;
            case 2:
                nowPlayingPlayer.transform.localPosition = new Vector3(240, 0 , 0);
                break;
            case 3:
                nowPlayingPlayer.transform.localPosition = new Vector3(720, 0 , 0);
                break;
        }
    }
    
    private void UpdateBottomBar()
    {
        var game = GameManager.Instance.Game;
        var currentPlayerId = AuthenticationManager.Instance.user.Id;
        var bottomBarTransform = bottomBar.transform;

        if (game == null) return;

        for (var i = 0; i < 4; i++)
        {
            var nameTextTransform = bottomBarTransform.GetChild(i).GetChild(0);
            var balanceTextTransform = bottomBarTransform.GetChild(i).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var balanceTextMeshPro = balanceTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = "";
            balanceTextMeshPro.text = "";
        }

        foreach (var player in game.Players)
        {
            var index = player.Index;
        
            var nameTextTransform = bottomBarTransform.GetChild(index).GetChild(0);
            var balanceTextTransform = bottomBarTransform.GetChild(index).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var balanceTextMeshPro = balanceTextTransform.GetComponent<TextMeshProUGUI>();
            

            if (player.UserId == currentPlayerId)
            {
                nameTextMeshPro.text = "•" + player.Name + "•";
            }
            else
            {
                nameTextMeshPro.text = player.Name;
            }
            
            balanceTextMeshPro.text = player.Balance + "ΔΜ";
        }
    }
}
