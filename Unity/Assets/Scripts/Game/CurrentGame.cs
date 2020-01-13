using System.Collections.Generic;
using System.Linq;
using Schema;
using TMPro;
using UnityEngine;

public class CurrentGame : MonoBehaviour
{
    public SocketIo socketIo;
    public GameObject bottomBar;
    public GameObject[] playerPrefabs = new GameObject [4];
    public GameObject GoNode;
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
        socketIo.PlayerTurnChanged += SocketIoOnPlayerTurnChanged;

        UpdateBottomBar();
        SetupPlayers();
        playerList = GameObject.FindGameObjectsWithTag("Player").ToList();
        playerList.Reverse();
        
        CameraController.SetUpCameras();
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
        StartCoroutine(diceContainer.RollTheDice(dice));
    }

    private void SocketIoOnPlayerTurnChanged(Player player)
    {
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
        var newPlayer = Instantiate(playerPrefabs[player.Index], playerPos, Quaternion.identity);
        newPlayer.GetComponent<PlayerMovement>().offset = _offsets[player.Index];
    }
    
    private void UpdateBottomBar()
    {
        var game = GameManager.Instance.Game;
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

            nameTextMeshPro.text = player.UserId;
            balanceTextMeshPro.text = player.Balance + "ΔΜ";
        }
    }
}
