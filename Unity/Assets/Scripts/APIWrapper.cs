using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class APIWrapper : MonoBehaviour
{
    #region Singleton
    public static APIWrapper Instance { get => _instance; }

    private static APIWrapper _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public delegate void APICallback(JToken response, string error = null);
    
    public void AuthLogin(string username, string password, APICallback callback = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        StartCoroutine(Upload("auth/login", form, callback));
    }

    public void AuthLogout(APICallback callback = null)
    {
        StartCoroutine(Upload("auth/logout", null, callback));
    }
    
    public void AuthRegister(string username, string password, APICallback callback = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        StartCoroutine(Upload("auth/register", form, callback));
    }

    public void AuthSession(APICallback callback = null)
    {
        StartCoroutine(Upload("auth/session", null, callback));
    }
    
    public void GameNew(int seats, APICallback callback = null) => GameNew(seats, false, callback);

    public void GameNew(int seats, bool inviteOnly, APICallback callback = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("seats", seats);
        if (inviteOnly)
        {
            form.AddField("invite_only", "true");
        }

        StartCoroutine(Upload("game/new", form, callback));
    }

    public void GameJoin(string gameId, APICallback callback = null) => GameJoin(gameId, "", callback);

    public void GameJoin(string gameId, string invitationCode, APICallback callback = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", gameId);
        if (!string.IsNullOrEmpty(invitationCode))
        {
            form.AddField("invitation_code", invitationCode);
        }

        StartCoroutine(Upload("game/join", form, callback));
    }

    public void GameList(APICallback callback = null)
    {
        StartCoroutine(Upload("game/list", null, callback));
    }

    public void GameCurrent(APICallback callback = null)
    {
        StartCoroutine(Upload("game/current", null, callback));
    }
    
    public void GameLeave(APICallback callback = null)
    {
        StartCoroutine(Upload("game/leave", null, callback));
    }
    
    public void GamePrices(APICallback callback = null)
    {
        StartCoroutine(Upload("game/prices", null, callback));
    }
    
    public void PlayerRollDice(APICallback callback = null)
    {
        StartCoroutine(Upload("player/roll-dice", null, callback));
    }

    private IEnumerator Upload(string path, WWWForm form = null, APICallback callback = null)
    {
         // TODO: Hardcoded URL
        using (UnityWebRequest www = 
            form == null 
                ? UnityWebRequest.Get("http://localhost:3000/" + path)
                : UnityWebRequest.Post("http://localhost:3000/" + path, form))
        {
            yield return www.SendWebRequest();

            if (callback != null)
            {
                try
                {
                    string resText = www.downloadHandler.text;
                    JToken response = JToken.Parse(resText);
                    callback(response, www.error);
                }
                catch (JsonException ex)
                {
                    callback(null, ex.Message);
                }
            }
        }
    }
}
