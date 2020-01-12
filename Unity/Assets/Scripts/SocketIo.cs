using System;
using System.Collections;
using System.Text;
using System.Threading;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Schema;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnPlayerJoined(Player player);
public delegate void OnPlayerLeft(Player player);
public delegate void OnGameStarted();

public class SocketIo : MonoBehaviour
{
    private WebSocket _websocket;
    private bool _closed;
    public event OnPlayerJoined PlayerJoined;
    public event OnPlayerLeft PlayerLeft;
    public event OnGameStarted GameStarted;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Upload("socket.io/?EIO=3&transport=polling", null, async (response, error) =>
        {
            if (error != null) return;
            
            string sid = (string) response["sid"];

            _websocket = new WebSocket("ws://localhost:3000/socket.io/?EIO=3&transport=websocket&sid=" + sid);

            _websocket.OnOpen += () =>
            {
                _closed = false;
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
                if (!_closed) StartCoroutine(waiter());
            };

            _websocket.OnMessage += (bytes) =>
            {
                if (_closed) return;
                
                try
                {
                    string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    if (message == "3probe")
                    {
                        _websocket.SendText("5");
                        return;
                    }

                    if (message.Length <= 3 || message.Substring(0, 2) != "42") return;
                    
                    message = message.Substring(2);
                    JArray array = JArray.Parse(message);
                    
                    switch ((string) array[0])
                    {
                        case "playerJoined":
                        {
                            Player player = Player.GetPlayer(array[1]["player"]);
                            GetCurrentGame().Players.Add(player);
                            PlayerJoined?.Invoke(player);
                            break;
                        }
                        case "playerLeft":
                        {
                            foreach (var player in GetCurrentGame().Players.ToArray())
                            {
                                if (player.UserId == (string) array[1]["user"])
                                {
                                    GetCurrentGame().Players.Remove(player);
                                    PlayerLeft?.Invoke(player);
                                }
                            }
                            break;
                        }
                        case "gameStarted":
                        {
                            GameManager.Instance.Game.SetRunning();
                            GameStarted?.Invoke();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            };

            // Keep sending messages every 15s
            InvokeRepeating(nameof(SendWebSocketMessage), 0.0f, 5.0f);
            
            // Waiting for messages
            await _websocket.Connect();
        }));
    }
    
    private void OnDestroy()
    {
        Close();
    }
    
    IEnumerator waiter()
    {
        yield return new WaitForSecondsRealtime(5);
        Start();
    }

    async void SendWebSocketMessage()
    {
        if (_websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            await _websocket.SendText("2");
        }
    }

    public void Close()
    {
        try
        {
            _closed = true;
            CancelInvoke(nameof(SendWebSocketMessage));
            _websocket.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }

    private Game GetCurrentGame()
    {
        return GameManager.Instance.Game;
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
                    string json = resText.Split(new[] {":0"}, StringSplitOptions.None)[1];
                    JToken response = JToken.Parse(json);
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
