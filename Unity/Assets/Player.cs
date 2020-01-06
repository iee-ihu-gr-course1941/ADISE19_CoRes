using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public CameraController cameraController;
    void Start()
    {
        cameraController = GameObject.FindWithTag("GameController").GetComponent<CameraController>();
        cameraController.cameraList.Insert(cameraController.cameraList.Count, transform.Find("VirtualCamera").gameObject);
    }
}
