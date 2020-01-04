using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PriceLoader : MonoBehaviour
{
    public JToken prices;
    public Properties properties;
    public Taxes taxes;

    private void Start()
    {
        APIWrapper.Instance.GamePrices((response, error) =>
        {
            if (error == null)
            {
                JArray propertyPricesArray = (JArray) response["properties"];
                List<int> propertyPrices = new List<int>(propertyPricesArray.Count);
                foreach (var price in propertyPricesArray)
                {
                    int p = (int) price["price"];
                    propertyPrices.Add(p);
                }
                
                JArray taxPricesArray  = (JArray) response["taxes"];
                List<int> taxPrices = new List<int>(taxPricesArray.Count);
                foreach (var price in taxPricesArray)
                {
                    int p = (int) price["price"];
                    taxPrices.Add(p);
                }

                prices = response;

                LoadPrices(propertyPrices, taxPrices);
            }
            else
            {
                throw new Exception(error);
            }
        });
    }

   private void LoadPrices(List<int> propertyPrices, List<int> taxPrices)
    {
        for (int i = 0; i < propertyPrices.Count; i++)
        {
            properties.pricesList[i].text = propertyPrices[i] + "ΔΜ";
        }
        
        for (int i = 0; i < taxPrices.Count; i++)
        {
            taxes.pricesList[i].text = taxPrices[i] + "ΔΜ";
        }
    }
}
