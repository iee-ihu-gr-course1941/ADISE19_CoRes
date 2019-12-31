using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

using Schema;

public class NewGameDialog : MonoBehaviour
{
    public GameObject canvas;
    public GameObject create;
    public GameObject seats;

    private void Awake()
    {
        Button canvasBtn = canvas.GetComponent<Button>();
        canvasBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        TextInput seatsTxt = seats.GetComponent<TextInput>();
        Button createBtn = create.GetComponent<Button>();
        createBtn.onClick.AddListener(() =>
        {
            seatsTxt.Highlight = false;
            
            int seatsNumber;
            if (int.TryParse(seatsTxt.Value, out seatsNumber) && seatsNumber > 0)
            {
                APIWrapper.Instance.GameNew(seatsNumber, (response, error) =>
                {
                    if (error != null)
                    {
                        if (response["error"] != null)
                        {
                            Debug.Log((string) response["error"]["message"]);
                        }
                        else if (response["errors"] != null)
                        {
                            JArray errors = (JArray) response["errors"];
                            foreach (JToken err in errors.Children())
                            {
                                if ((string) err["msg"] == "Invalid value")
                                {
                                    if ((string) err["param"] == "seats")
                                    {
                                        seatsTxt.Highlight = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Game game = Game.GetGame(response);
                        GameManager.Instance.GoToGame(game);
                    }
                });
            }
            else
            {
                seatsTxt.Highlight = true;
            }
        });
    }
}
