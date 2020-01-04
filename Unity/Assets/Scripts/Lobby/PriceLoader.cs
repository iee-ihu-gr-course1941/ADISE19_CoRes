using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PriceLoader : MonoBehaviour
{
    public List<int> propertiesPrices;
    public List<int> taxesPrices;
    public JArray prices;
    public Properties properties;
    public Taxes taxes;

    private void Start()
    {
        APIWrapper.Instance.GamePrices((response, error) =>
        {
            if (error == null)
            {
                prices = (JArray) response["properties"];
                propertiesPrices = new List<int>(prices.Count);
                foreach (var price in prices)
                {
                    int p = (int) price["price"];
                    propertiesPrices.Add(p);
                    Debug.Log(p);
                }
                prices = (JArray) response["taxes"];
                taxesPrices = new List<int>(prices.Count);
                foreach (var price in prices)
                {
                    int p = (int) price["price"];
                    taxesPrices.Add(p);
                    Debug.Log(p);
                }
                LoadPrices();
            }
            else
            {
                throw new Exception(error);
            }
        });
    }

   public void LoadPrices()
    {
        for (int i = 0; i < propertiesPrices.Count; i++)
        {
            properties.pricesList[i].text = propertiesPrices[i] + "ΔΜ";
        }
        for (int i = 0; i < taxesPrices.Count; i++)
        {
            taxes.pricesList[i].text = taxesPrices[i] + "ΔΜ";
        }
    }
}
