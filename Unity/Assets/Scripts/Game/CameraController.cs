using System.Collections.Generic;
using Schema;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Game _game;
    public List<GameObject> cameraList;
    public CurrentGame currentGame;

    private void Start()
    {
        currentGame = GameObject.Find("CurrentGame").GetComponent<CurrentGame>();
        _game = GameManager.Instance.Game;
    }

    public void SetUpCameras()
    {
        foreach (GameObject player in currentGame.playerList)
        {
            cameraList.Add(player.transform.Find("VirtualCamera").gameObject);
        }
        
        foreach (var vCamera in cameraList)
        {
            vCamera.gameObject.SetActive(false);
        }

        if (cameraList.Count > 0)
        {
            var currentPlayerId = _game.CurrentPlayerId;
            var index = Player.GetPlayerById(currentPlayerId).Index;
            cameraList[index].gameObject.SetActive(true);
        }
    }

    public void FocusCameraOn(Player player)
    {
        for (int i = 0; i < cameraList.Count; i++)
        {
            cameraList[i].gameObject.SetActive(i == player.Index);
        }
    }
}
