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
