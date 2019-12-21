using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGames : MonoBehaviour
{
    public List<string> gameList;
    public Text noGamesFoundText;
    public GameObject mainScrollContentView;
    public GameObject contentDataPanel;
    // Start is called before the first frame update
    void Start()
    {
        SetListData();
        Initialize();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    void SetListData()
    {
        gameList = new List<string>();
        for (int i = 0; i < 20; i++)
        {
            gameList.Add("Bruh" + i);   
        }
    }

    void Initialize()
    {
        if (gameList.Count > 0) {
            HideNoGamesFoundText();
            RectTransform rt = (RectTransform) mainScrollContentView.transform;
            foreach (var game in gameList)
            {
                GameObject playerTextPanel = Instantiate(contentDataPanel, mainScrollContentView.transform, true);
                playerTextPanel.transform.localScale = new Vector3(1,1,1);
                playerTextPanel.transform.localPosition = new Vector3(0,0,0);
                playerTextPanel.transform.Find("Text").GetComponent<Text>().text = game;
            }
        }
        else
        {
            ShowNoGamesFoundText();
        }
    }

    void ShowNoGamesFoundText()
    {
        noGamesFoundText.gameObject.SetActive(true);
    }
    
    void HideNoGamesFoundText()
    {
        noGamesFoundText.gameObject.SetActive(false);
    }
}
