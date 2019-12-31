using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

using Schema;

public class LoadGames : MonoBehaviour
{
    public List<Game> gameList;
    public Text noGamesFoundText;
    public GameObject mainScrollContentView;
    public GameObject contentDataPanel;
    // Start is called before the first frame update
    void Start()
    {
        APIWrapper.Instance.GameList((response, error) =>
        {
            if (error == null)
            {
                JArray games = (JArray) response;
                gameList = new List<Game>(games.Count);
                foreach (JToken game in games)
                {
                    gameList.Add(Game.GetGame(game));
                }
                Initialize();
            }
            else
            {
                Debug.Log(error);
            }
        });
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
                playerTextPanel.transform.Find("Text").GetComponent<Text>().text = game.SeatsToString();
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
