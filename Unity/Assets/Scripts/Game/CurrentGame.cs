using System.Collections;
using System.Collections.Generic;
using Schema;
using TMPro;
using UnityEngine;

public class CurrentGame : MonoBehaviour
{
    private CoroutineQueue _queue;
    private CameraController _cameraController;
    public SocketIo socketIo;
    public GameObject bottomBar;
    public TextMeshProUGUI statusMessage;
    public GameObject[] playerPrefabs = new GameObject[4];
    public Route route;
    public GameObject players;
    public GameObject nowPlayingPlayer;
    private CardLoader _cardLoader;
    public Dice diceContainer;
    public GameObject chanceCard;
    public GameObject communityChestCard;
    private BuyProperty _buyProperty;

    private readonly Vector3[] _offsets = {
        new Vector3(0.15f, 0, 0.15f),
        new Vector3(-0.15f, 0, 0.15f), 
        new Vector3(0.15f, 0, -0.15f),
        new Vector3(-0.15f, 0, -0.15f)
    };
    
    public List<GameObject> playerList;

    private void Start()
    {
        _queue = new CoroutineQueue(this);
        _queue.StartLoop();
        
        _queue.EnqueueAction(ShowStatusMessage("Press space to roll the dice"));
        
        _cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        _buyProperty = GameObject.Find("BuyUI").GetComponent<BuyProperty>();
        _cardLoader = GameObject.Find("CardLoader").GetComponent<CardLoader>();

        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;
        socketIo.PlayerRolledDice += SocketIoOnPlayerRolledDice;
        socketIo.PlayerMoved+= SocketIoOnPlayerMoved;
        socketIo.PlayerTurnChanged += SocketIoOnPlayerTurnChanged;
        socketIo.PlayerPlaysAgain += SocketIoOnPlayerPlaysAgain;
        socketIo.PlayerSteppedOnChance += SocketIoOnPlayerSteppedOnChance;
        socketIo.PlayerSteppedOnCommunityChest += SocketIoOnPlayerSteppedOnCommunityChest;

        UpdateBottomBar();
        SetupPlayers();

        foreach (Transform child in players.transform)
        {
            if (child != players.transform)
            {
                playerList.Add(child.gameObject);
            }
        }

        var player = Player.GetPlayerById(GameManager.Instance.Game.CurrentPlayerId);
        UpdateBottomBarPlayerPlaying(player);
        
        _cameraController.SetUpCameras();
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
        _queue.EnqueueAction(diceContainer.RollTheDice(dice));
        _queue.EnqueueWait(1f);
    }
    
    private void SocketIoOnPlayerMoved(Player player, int location)
    {
        var playerObject = playerList[player.Index];
        _queue.EnqueueAction(playerObject.GetComponent<PlayerMovement>().Move(location));
        _queue.EnqueueWait(1f);
        if (Property.GetPropertyByLocation(location) != null)
        {
            _queue.EnqueueAction(_buyProperty.DisplayCard(location));
        }
    }
    
    private void SocketIoOnPlayerTurnChanged(Player player)
    {
        _queue.EnqueueAction(ShowStatusMessage("It's " + player.Name + "'s turn"));

        UpdateBottomBarPlayerPlaying(player);
        _cameraController.FocusCameraOn(player);
    }

    private void SocketIoOnPlayerPlaysAgain(Player player)
    {
        _queue.EnqueueAction(ShowStatusMessage(player.Name + " plays again"));
    }

    private void SocketIoOnPlayerSteppedOnChance(Player player, string text)
    {
        _queue.EnqueueAction(DisplayChanceCard(text));
        _queue.EnqueueWait(1f);
    }
    
    private void SocketIoOnPlayerSteppedOnCommunityChest(Player player, string text)
    {
        _queue.EnqueueAction(DisplayCommunityChestCard(text));
        _queue.EnqueueWait(1f);
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
        var playerPos = route.childNodeList[player.Position].transform.position + _offsets[player.Index];
        var playerRotation = new Vector3(0, player.Position / 10 * 90, 0);
        var newPlayer = Instantiate(playerPrefabs[player.Index], playerPos, Quaternion.Euler(playerRotation), players.transform);
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
        var selfPlayerId = AuthenticationManager.Instance.user.Id;
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
            

            if (player.UserId == selfPlayerId)
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
    
    private IEnumerator ShowStatusMessage(string text)
    {
        statusMessage.text = text;

        yield return new WaitForSecondsRealtime(2f);
        
        statusMessage.text = "";
        
        chanceCard.SetActive(false);
    }

    private IEnumerator DisplayChanceCard(string text)
    {
        chanceCard.SetActive(true);
        chanceCard.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = text;
        yield return new WaitForSecondsRealtime(5f);
        chanceCard.SetActive(false);
    }
    
    private IEnumerator DisplayCommunityChestCard(string text)
    {
        communityChestCard.SetActive(true);
        communityChestCard.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = text;
        yield return new WaitForSecondsRealtime(5f);
        communityChestCard.SetActive(false);
    }
}
