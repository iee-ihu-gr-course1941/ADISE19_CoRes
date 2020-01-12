using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    public List<PropertyCard> propertiesList;
    public List<StationCard> stationsList;
    public List<UtilityCard> utilitiesList;
    
    private void Start()
    {
        APIWrapper.Instance.GamePrices((response, error) =>
        {
            if (error == null)
            {
                var propertyPricesArray = (JArray) response["properties"];
                propertiesList = new List<PropertyCard>(propertyPricesArray.Count);
                stationsList = new List<StationCard>(propertyPricesArray.Count);
                utilitiesList = new List<UtilityCard>(propertyPricesArray.Count);
                foreach (var property in propertyPricesArray)
                {
                    var name = property["name"].ToString();
                    var color = property["color"]?.ToString();
                    var location = (int) property["location"];
                    var rents = ((JArray) property["rents"]).Select(rent => (int) rent).ToArray();
                    var mortgage = (int) property["mortgage"];

                    var type = (int) property["type"];
                    switch (type)
                    {
                        case 0:
                        {
                            var houseCosts = (JArray) response["houses"];
                        
                            var houseCost = (int) houseCosts[location / 10];
                            
                            var card = new PropertyCard(name, color, location, rents, houseCost, mortgage);
                            propertiesList.Add(card);
                            break;
                        }
                        case 1:
                        {
                            var card = new StationCard(name, location, rents, mortgage);
                            stationsList.Add(card);
                            break;
                        }
                        default:
                        {
                            var card = new UtilityCard(name, location, rents, mortgage);
                            utilitiesList.Add(card);
                            break;
                        }
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