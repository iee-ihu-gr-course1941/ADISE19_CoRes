using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyProperty : MonoBehaviour
{
    private GameObject _abandon;
    private GameObject _buy;
    private GameObject _cards;
    public GameObject CardLoader;
    private CardLoader _cardLoader;
    private List<GameObject> _cardsList;
    void Start()
    {
        _abandon = transform.Find("Abandon").gameObject;
        _buy = transform.Find("Buy").gameObject;
        _cards = transform.Find("Cards").gameObject;
        _cardsList = new List<GameObject>();
        _cardLoader = CardLoader.GetComponent<CardLoader>();
        foreach (Transform child in _cards.transform)
        {
            _cardsList.Add(child.gameObject);
        }
    }

    public IEnumerator DisplayCard(int location)
    {
        _abandon.SetActive(true);
        _buy.SetActive(true);
        if (location == 5 || location == 15 || location == 25 || location == 35)
        {
            List<StationCard> stationCards = _cardLoader.stationsList;
            StationCard stationCard = stationCards[0];
            foreach (StationCard card in stationCards)
            {
                if (card.location == location)
                { 
                    stationCard = card;
                }
            }
            _cards.SetActive(true);
            GameObject stCard = _cardsList[1];
            stCard.SetActive(true);
            stCard.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = stationCard.name;
            for (int i = 0; i < stationCard.rents.Length; i++)
            {
                stCard.transform.Find("Rent" + i).gameObject.GetComponent<TextMeshProUGUI>().text =
                    stationCard.rents[i].ToString();
            }
            stCard.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = stationCard.mortgage.ToString();
        }
        if (location == 12)
        {
            List<UtilityCard> utilityCards = _cardLoader.utilitiesList;
            UtilityCard utilityCard = utilityCards[0];
            _cards.SetActive(true);
            GameObject uCard1 = _cardsList[2];
            uCard1.SetActive(true);
            uCard1.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = utilityCard.mortgage.ToString();
        }
        if (location == 28)
        {
            List<UtilityCard> utilityCards = _cardLoader.utilitiesList;
            UtilityCard utilityCard = utilityCards[0];
            _cards.SetActive(true);
            GameObject uCard2 = _cardsList[3];
            uCard2.SetActive(true);
            uCard2.transform.Find("MortgageText").gameObject.GetComponent<TextMeshProUGUI>().text = utilityCard.mortgage.ToString();
        }
        else
        {
            List<PropertyCard> propertyCards = _cardLoader.propertiesList;
            PropertyCard propertyCard = propertyCards[0];
            foreach (PropertyCard card in propertyCards)
            {
                if (card.location == location)
                { 
                    propertyCard = card;
                }
            }
            _cards.SetActive(true);
            foreach (GameObject card in _cardsList)
            {
                if (card.name != "PropertyCard")
                {
                    card.SetActive(false);
                }
                else
                {
                    card.SetActive(true);
                    Color color;
                    ColorUtility.TryParseHtmlString("#" + propertyCard.color, out color);
                    card.transform.Find("Colour").gameObject.GetComponent<Image>().color = color;
                    card.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = propertyCard.name;
                    for (int i = 0; i < propertyCard.rents.Length; i++)
                    {
                        card.transform.Find("Rent" + i).gameObject.GetComponent<TextMeshProUGUI>().text =
                            propertyCard.rents[i].ToString();
                    }
                    card.transform.Find("BookCostText").gameObject.GetComponent<TextMeshProUGUI>().text = propertyCard.houseCost.ToString();
                    card.transform.Find("DegreeCostText").gameObject.GetComponent<TextMeshProUGUI>().text =
                        propertyCard.houseCost.ToString();
                }
            }
        }
        yield return new WaitForSeconds(5f);
        Abandon();
    }
    
    public void Abandon()
    {
        StopCoroutine("DisplayCard");
        _abandon.SetActive(false);
        _buy.SetActive(false);
        foreach (GameObject child in _cardsList)
        {
            child.SetActive(false);
        }
        _cards.SetActive(false);
    }

    public void Buy()
    {
        APIWrapper.Instance.TransactionBuyCurrentProperty((response, error) =>
        {
            if (error != null)
            {
                Debug.Log(error); 
            }
            else
            {
                StopCoroutine("DisplayCard");
                _abandon.SetActive(false);
                _buy.SetActive(false);
                foreach (GameObject child in _cardsList)
                {
                    child.SetActive(false);
                }
                _cards.SetActive(false);
            }
        });
    }
}
