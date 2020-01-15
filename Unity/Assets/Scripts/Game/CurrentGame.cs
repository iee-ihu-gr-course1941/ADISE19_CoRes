using System.Collections;
using System.Collections.Generic;
using Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentGame : MonoBehaviour
{
    private CoroutineQueue _queue;
    public SocketIo socketIo;
    public GameObject ownedProperties;
    public GameObject bottomBar;
    public TextMeshProUGUI statusMessage;
    public GameObject[] playerPrefabs = new GameObject[4];
    public Route route;
    public GameObject players;
    public GameObject nowPlayingPlayer;
    public CameraController cameraController;
    public Dice diceContainer;
    public GameObject chanceCard;
    public GameObject communityChestCard;

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
        
        cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;
        socketIo.PlayerRolledDice += SocketIoOnPlayerRolledDice;
        socketIo.PlayerMoved+= SocketIoOnPlayerMoved;
        socketIo.PlayerTurnChanged += SocketIoOnPlayerTurnChanged;
        socketIo.PlayerPlaysAgain += SocketIoOnPlayerPlaysAgain;
        socketIo.PlayerSteppedOnChance += SocketIoOnPlayerSteppedOnChance;
        socketIo.PlayerSteppedOnCommunityChest += SocketIoOnPlayerSteppedOnCommunityChest;
        socketIo.PropertyOwnerChanged += SocketIoOnPropertyOwnerChanged;

        UpdateOwnedProperties();
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
        
        cameraController.SetUpCameras();
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
    }
    
    private void SocketIoOnPlayerTurnChanged(Player player)
    {
        _queue.EnqueueAction(ShowStatusMessage("It's " + player.Name + "'s turn"));

        UpdateBottomBarPlayerPlaying(player);
        cameraController.FocusCameraOn(player);
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
    
    private void SocketIoOnPropertyOwnerChanged(int propertyIndex, int ownerId)
    {
        UpdateOwnedProperties();
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

    private void UpdateOwnedProperties()
    {
        var game = GameManager.Instance.Game;
        var properties = GameManager.Instance.Game.Properties;

        foreach (var player in game.Players)
        {
            var playerOwnedProperties = ownedProperties.transform.GetChild(player.Index);
            playerOwnedProperties.gameObject.SetActive(true);
            
            foreach (var property in properties)
            {
                var propertyComponent = playerOwnedProperties.Find(property.Location.ToString());
                
                if (propertyComponent == null) return;
                
                var image = propertyComponent.GetComponent<Image>();
                var tempColor = image.color;
                tempColor.a = property.OwnerId != null ? 1f : 0.5f;
                image.color = tempColor;
            }
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
