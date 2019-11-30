using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Login : MonoBehaviour
{
    public TextInput username;
    public TextInput password;
    public Submit submit;
    public Status statusMessage;

    private void Awake()
    {
        submit.Click += delegate(object sender, EventArgs args)
        {
            username.Highlight = false;
            password.Highlight = false;
            statusMessage.Value = "";
            
            AuthenticationManager.Instance.LoginFormSubmit(username.Value, password.Value, (response, error) =>
            {
                if (error != null)
                {
                    if (response["error"] != null)
                    {
                        statusMessage.Value = (string)response["error"]["message"];
                    }
                    else if (response["errors"] != null) 
                    {
                        StringBuilder sb = new StringBuilder();
                        JArray errors = (JArray) response["errors"];
                        foreach (JToken err in errors.Children())
                        {
                            if ((string)err["msg"] == "Invalid value")
                            {
                                if ((string) err["param"] == "username")
                                {
                                    username.Highlight = true;
                                }
                                else if ((string) err["param"] == "password")
                                {
                                    password.Highlight = true;
                                }
                                else
                                {
                                    sb.AppendLine((string) err["msg"]);
                                }
                            }
                        }

                        if (sb.Length != 0)
                        {
                            statusMessage.Value = sb.ToString();
                        }
                    }
                    else
                    {
                        Debug.Log(error);
                    }
                }
            });
        };
    }
}
