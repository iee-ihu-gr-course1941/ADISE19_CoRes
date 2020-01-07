using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

using Schema;
using TMPro;
using Debug = UnityEngine.Debug;

public class LoadGames : MonoBehaviour
{
    public List<Game> gameList;
    public TextMeshProUGUI noGamesFoundText;
    public GameObject mainScrollContentView;
    public GameObject contentDataPanel;
    // Start is called before the first frame update
    void Start()
    {
        APIWrapper.Instance.GameCurrent((response, error) =>
        {
            if (error == null)
            {
                Game gameToJoin = Game.GetGame(response);
                GameManager.Instance.GoToGame(gameToJoin);
            }
            else
            {
                LoadGameList();
            }
        });
    }

    void Initialize()
    {
        if (gameList.Count > 0) {
            HideNoGamesFoundText();
            foreach (var game in gameList)
            {
                GameObject playerTextPanel = Instantiate(contentDataPanel, mainScrollContentView.transform, true);
                playerTextPanel.transform.localScale = new Vector3(1,1,1);
                playerTextPanel.transform.localPosition = new Vector3(0,0,0);
                playerTextPanel.transform.Find("WaitingText").GetComponent<TextMeshProUGUI>().text = "Waiting for players " + game.SeatsToString();
                playerTextPanel.transform.Find("Join").GetComponent<Submit>().Click += (sender, args) =>
                {
                    APIWrapper.Instance.GameJoin(game.Id, (response, error) =>
                    {
                        if (error != null)
                        {
                            if (response["error"] != null)
                            {
                                Debug.Log((string) response["error"]["message"]);
                            }
                        }
                        else
                        {
                            Game gameToJoin = Game.GetGame(response);
                            GameManager.Instance.GoToGame(gameToJoin);
                        }
                    });
                };
            }
        }
        else
        {
            ShowNoGamesFoundText();
        }
    }
    
    public void LoadGameList()
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

    void ShowNoGamesFoundText()
    {
        noGamesFoundText.gameObject.SetActive(true);
    }
    
    void HideNoGamesFoundText()
    {
        noGamesFoundText.gameObject.SetActive(false);
    }
}
