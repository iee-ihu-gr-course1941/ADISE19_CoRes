using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    public List<PropertyCard> propertiesList;
    public List<StationCard> stationsList;
    public List<UtilityCard> utilitysList;
    
    private void Start()
    {
        APIWrapper.Instance.GamePrices((response, error) =>
        {
            if (error == null)
            {
                JArray propertyPricesArray = (JArray) response["properties"];
                propertiesList = new List<PropertyCard>(propertyPricesArray.Count);
                stationsList = new List<StationCard>(propertyPricesArray.Count);
                utilitysList = new List<UtilityCard>(propertyPricesArray.Count);
                foreach (var property in propertyPricesArray)
                {
                    if ((int) property["type"] == 0)
                    {
                        JArray houseCosts = (JArray) response["houses"];
                        int location = (int) property["location"];
                        int houseCost;
                        switch (location / 10)
                        {
                            case 0:
                                 houseCost = (int) houseCosts[0];
                                break;
                            case 1:
                                 houseCost = (int) houseCosts[1];
                                break;
                            case 2:
                                 houseCost = (int) houseCosts[2];
                                break;
                            case 3:
                                 houseCost = (int) houseCosts[3];
                                break;
                            default:
                                houseCost = 0;
                                break;
                        }
                        PropertyCard card = new PropertyCard(
                            property["name"].ToString(),
                            property["color"].ToString(),
                            location,
                             property["rents"].ToObject<int[]>(),
                            houseCost,
                            (int) property["mortgage"]
                            );
                        propertiesList.Add(card);
                    }
                    else if ((int) property["type"] == 1)
                    {
                        StationCard card = new StationCard(
                            property["name"].ToString(),
                            (int) property["location"],
                            property["rents"].ToObject<int[]>(),
                            (int) property["mortgage"]
                        );
                        stationsList.Add(card);
                    }
                    else
                    {
                        UtilityCard card = new UtilityCard(
                            property["name"].ToString(),
                            (int) property["location"],
                            property["rents"].ToObject<int[]>(),
                            (int) property["mortgage"]
                        );
                        utilitysList.Add(card);
                    }
                }
            }
            else
            {
                throw new Exception(error);
            }
        });
    }
}