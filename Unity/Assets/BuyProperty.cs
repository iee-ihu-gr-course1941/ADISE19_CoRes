using System;
using System.Collections;
using System.Collections.Generic;
using Schema;
using UnityEngine;

public class BuyProperty : MonoBehaviour
{
    private GameObject _abandon;
    private GameObject _buy;
    private GameObject _cards;
    private CardLoader _cardLoader;
    private List<GameObject> _cardsList;
    void Start()
    {
        _abandon = transform.Find("Abandon").gameObject;
        _buy = transform.Find("Buy").gameObject;
        _cards = transform.Find("Cards").gameObject;
        _cardsList = new List<GameObject>();
        foreach (Transform child in _cards.transform)
        {
            _cardsList.Add(child.gameObject);
        }
    }

    public IEnumerator DisplayCard(int location)
    {
        _abandon.SetActive(true);
        _buy.SetActive(true);
        if (Array.Exists(new [] {5, 15, 25, 35}, i => i == location ))
        {
            _cards.SetActive(true);
            foreach (GameObject card in _cardsList)
            {
                if (card.name != "StationCard")
                {
                    card.SetActive(false);
                }
            }
        }
        if (Array.Exists(new [] {12}, i => i == location ))
        {
            _cards.SetActive(true);
            foreach (GameObject card in _cardsList)
            {
                if (card.name != "UtilityCard1")
                {
                    card.SetActive(false);
                }
            }
        }
        if (Array.Exists(new [] {28}, i => i == location ))
        {
            _cards.SetActive(true);
            foreach (GameObject card in _cardsList)
            {
                if (card.name != "UtilityCard2")
                {
                    card.SetActive(false);
                }
            }
        }
        else
        {
            _cards.SetActive(true);
            _cardsList[0].SetActive(true);
        }
        yield return new WaitForSeconds(10f);
        Abandon();
    }
    
    public void Abandon()
    {
        StopCoroutine("DisplayCard");
        _abandon.SetActive(false);
        _buy.SetActive(false);
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
                _cards.SetActive(false);
            }
        });
    }
}
