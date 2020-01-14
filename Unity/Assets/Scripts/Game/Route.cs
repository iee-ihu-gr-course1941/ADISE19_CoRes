using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    private Transform[] childObjects;
    public List<Transform> childNodeList = new List<Transform>();

    private void Start()
    {
        FillNodes();
    }

    private void FillNodes()
    {
        childNodeList.Clear();

        childObjects = GetComponentsInChildren<Transform>();

        foreach (Transform child in childObjects)
        {
            if (child != transform)
            {
                childNodeList.Add(child);
            }
        }
    }
}
