﻿using Schema;
using TMPro;
using UnityEngine;

public class CurrentLobby : MonoBehaviour
{
    public SocketIo socketIo;
    public GameObject waitingText;
    public GameObject bottomBar;
    
    // Start is called before the first frame update
    void Start()
    {
        socketIo.PlayerJoined += SocketIoOnPlayerJoined;
        socketIo.PlayerLeft += SocketIoOnPlayerLeft;
        
        UpdateWaitingText();
        UpdateBottomBar();
    }

    private void SocketIoOnPlayerJoined(Player player)
    {
        UpdateWaitingText();
        UpdateBottomBar();
    }
    
    private void SocketIoOnPlayerLeft(Player player)
    {
        UpdateBottomBar();
    }

    private void UpdateWaitingText()
    {
        var game = GameManager.Instance.Game;
        var waitingTextTransform = waitingText.transform;
        var waitingTextMeshPro = waitingTextTransform.GetComponent<TextMeshProUGUI>();
        waitingTextMeshPro.text = "Waiting for players " + game.Players.Count + "/" + game.Seats;
    }
    
    private void UpdateBottomBar()
    {
        var game = GameManager.Instance.Game;
        var bottomBarTransform = bottomBar.transform;

        if (game == null) return;

        for (var i = 0; i < 4; i++)
        {
            var nameTextTransform = bottomBarTransform.GetChild(i).GetChild(0);
            var typeTextTransform = bottomBarTransform.GetChild(i).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var typeTextMeshPro = typeTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = "";
            typeTextMeshPro.text = "";
        }

        foreach (var player in game.Players)
        {
            var index = player.Index;
        
            var nameTextTransform = bottomBarTransform.GetChild(index).GetChild(0);
            var typeTextTransform = bottomBarTransform.GetChild(index).GetChild(1);
            var nameTextMeshPro = nameTextTransform.GetComponent<TextMeshProUGUI>();
            var typeTextMeshPro = typeTextTransform.GetComponent<TextMeshProUGUI>();

            nameTextMeshPro.text = player.UserId;
            typeTextMeshPro.text = "Player";
        }
    }
}
