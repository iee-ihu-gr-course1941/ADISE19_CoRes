using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Schema;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> cameraList;
    public List<GameObject> playerList;

    public void SetUpCameras()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player").ToList();
        playerList.Reverse();

        foreach (GameObject player in playerList)
        {
            cameraList.Add(player.transform.Find("VirtualCamera").gameObject);
        }
        
        foreach (var vCamera in cameraList)
        {
            vCamera.gameObject.SetActive(false);
        }

        if (cameraList.Count > 0){
            cameraList[0].gameObject.SetActive (true);
        }
    }

    /*void Update () {
        if (Input.GetKey(KeyCode.C)){
            currentCamera ++;
            if (currentCamera < cameraList.Count){
                cameraList[currentCamera - 1].gameObject.SetActive(false);
                cameraList[currentCamera].gameObject.SetActive(true);
            }
            else {
                cameraList[currentCamera - 1].gameObject.SetActive(false);
                currentCamera = 0;
                cameraList[currentCamera].gameObject.SetActive(true);
            }
        }
    }*/

    public void FocusCameraOn(Player player)
    {
        for (int i = 0; i < cameraList.Count; i++)
        {
            cameraList[i].gameObject.SetActive(i == player.Index);
        }
    }
}
