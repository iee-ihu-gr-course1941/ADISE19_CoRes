﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{
    #region Singleton
    public static AuthenticationManager Instance { get => _instance; }

    private static AuthenticationManager _instance;
    
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

    public string user;

    public void Start()
    {
        APIWrapper.Instance.AuthSession((response, error) =>
        {
            if (error == null)
            {
                user = (string)response["username"];
                SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
            else
            {
                if (response["error"] == null)
                {
                    throw new Exception(error);
                }
            }
        });
    }

    public void LoginFormSubmit(string username, string password, APIWrapper.APICallcack callback = null)
    {
        APIWrapper.Instance.AuthLogin(username, password, (response, error) =>
        {
            if (error == null)
            {
                user = (string)response["username"];
                SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
            else
            {
                if (callback != null)
                {
                    callback(response, error);
                }
                else
                {
                    throw new Exception(error);
                }
            }
        });
    }
    
    public void RegisterFormSubmit(string username, string password, APIWrapper.APICallcack callback = null)
    {
        APIWrapper.Instance.AuthRegister(username, password, (response, error) =>
        {
            if (error == null)
            {
                user = (string)response["username"];
                SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            }
            else
            {
                if (callback != null)
                {
                    callback(response, error);
                }
                else
                {
                    throw new Exception(error);
                }
            }
        });
    }

    public void Logout(APIWrapper.APICallcack callback = null)
    {
        APIWrapper.Instance.AuthLogout((response, error) =>
        {
            if (error == null)
            {
                user = "";
                SceneManager.LoadScene("Login", LoadSceneMode.Single);
            }
            else
            {
                if (callback != null)
                {
                    callback(response, error);
                }
                else
                {
                    throw new Exception(error);
                }
            }
        });
    }
}
