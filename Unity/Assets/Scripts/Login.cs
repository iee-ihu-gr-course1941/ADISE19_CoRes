using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Login : MonoBehaviour
{
    public TextInput username;
    public TextInput password;

    public void OnLogin()
    {
        Debug.Log(username.Value);
        Debug.Log(password.Value);
        
        StartCoroutine(Upload(ApiAction.Login));
    }

    public void OnLogout()
    {
        StartCoroutine(Upload(ApiAction.Logout));
    }
    
    IEnumerator Upload(ApiAction action)
    {
        switch (action)
        {
            case ApiAction.Login:
                WWWForm form = new WWWForm();
                form.AddField("username", username.Value);
                form.AddField("password", password.Value);

                using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:3000/auth/login", form))
                {
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        Debug.Log("Form upload complete!");
                    }
            
                    Debug.Log(www.downloadHandler.text);
                }
                break;
            case ApiAction.Logout:
                using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/auth/logout"))
                {
                    yield return www.SendWebRequest();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        Debug.Log("Form upload complete!");
                    }
            
                    Debug.Log(www.downloadHandler.text);
                }
                break;
            default:
                break;
        }
        
    }

    enum ApiAction
    {
        Login, Logout
    }
}
