using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SocketIo : MonoBehaviour
{
    private WebSocket _websocket;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Upload("socket.io/?EIO=3&transport=polling", null, async (response, error) =>
        {
            string sid = (string) response["data"]["sid"];

            _websocket = new WebSocket("ws://localhost:3000/socket.io/?EIO=3&transport=websocket&sid=" + sid);

            _websocket.OnOpen += () =>
            {
                Debug.Log("Connection open!");
                _websocket.SendText("2probe");
            };

            _websocket.OnError += (e) =>
            {
                Debug.Log("Error! " + e);
            };

            _websocket.OnClose += (e) =>
            {
                Debug.Log("Connection closed!");
            };

            _websocket.OnMessage += (bytes) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    if (message.Length > 3 && message.Substring(0, 2) == "42")
                    {
                        message = message.Substring(2);
                        JArray array = JArray.Parse(message);
            
                        if ((string) array[0] == "location")
                        {
                            JToken vector = array[1];
                            int x = (int) vector["x"];
                            int y = (int) vector["y"];
                            int z = (int) vector["z"];
                            transform.position = new Vector3(x, y, z);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            };

            // Keep sending messages every 15s
            InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 15.0f);

            // Waiting for messages
            await _websocket.Connect();
        }));
    }

    async void SendWebSocketMessage()
    {
        if (_websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            await _websocket.SendText("2");
        }
    }

    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }
    
    private IEnumerator Upload(string path, WWWForm form = null, APIWrapper.APICallback callback = null)
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
