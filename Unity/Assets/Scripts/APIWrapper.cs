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

    public delegate void APICallcack(JObject response, string error = null);
    
    public void AuthLogin(string username, string password, APICallcack callcack = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        StartCoroutine(Upload("auth/login", form, callcack));
    }

    public void AuthLogout(APICallcack callcack = null)
    {
        StartCoroutine(Upload("auth/logout", null, callcack));
    }
    
    public void AuthRegister(string username, string password, APICallcack callcack = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        StartCoroutine(Upload("auth/register", form, callcack));
    }

    public void AuthSession(APICallcack callcack = null)
    {
        StartCoroutine(Upload("auth/session", null, callcack));
    }
    
    public void GameNew(int seats, APICallcack callcack = null) => GameNew(seats, false, callcack);

    public void GameNew(int seats, bool inviteOnly, APICallcack callcack = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("seats", seats);
        if (inviteOnly)
        {
            form.AddField("invite_only", "true");
        }

        StartCoroutine(Upload("game/new", form, callcack));
    }

    public void GameJoin(string gameId, APICallcack callcack = null) => GameJoin(gameId, "", callcack);

    public void GameJoin(string gameId, string invitationCode, APICallcack callcack = null)
    {
        WWWForm form = new WWWForm();
        form.AddField("game_id", gameId);
        if (!string.IsNullOrEmpty(invitationCode))
        {
            form.AddField("invitation_code", invitationCode);
        }

        StartCoroutine(Upload("game/join", form, callcack));
    }

    public void GameList(APICallcack callcack = null)
    {
        StartCoroutine(Upload("game/list", null, callcack));
    }
    
    public void GameLeave(APICallcack callcack = null)
    {
        StartCoroutine(Upload("game/leave", null, callcack));
    }
    
    public void GamePrices(APICallcack callcack = null)
    {
        StartCoroutine(Upload("game/prices", null, callcack));
    }

    private IEnumerator Upload(string path, WWWForm form = null, APICallcack callcack = null)
    {
         // TODO: Hardcoded URL
        using (UnityWebRequest www = 
            form == null 
                ? UnityWebRequest.Get("http://localhost:3000/" + path)
                : UnityWebRequest.Post("http://localhost:3000/" + path, form))
        {
            yield return www.SendWebRequest();

            if (callcack != null)
            {
                try
                {
                    string resText = www.downloadHandler.text;
                    JObject response = JObject.Parse(resText);
                    callcack(response, www.error);
                }
                catch (JsonException ex)
                {
                    callcack(null, ex.Message);
                }
            }
        }
    }
}
