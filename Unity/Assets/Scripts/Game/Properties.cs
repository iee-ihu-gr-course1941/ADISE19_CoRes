using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties : MonoBehaviour
{
    private TextMesh[] prices;
    public List<TextMesh> pricesList = new List<TextMesh>();

    private void Start()
    {
        pricesList.Clear();

        prices = GetComponentsInChildren<TextMesh>();
        foreach (TextMesh textMesh in prices)
        {
            pricesList.Add(textMesh);
        }
    }
}
