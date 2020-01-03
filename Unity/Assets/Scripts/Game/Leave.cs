using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leave : MonoBehaviour
{
    public void OnClick()
    {
        APIWrapper.Instance.GameLeave((response, error) =>
        {
            if (error == null)
            {
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
}
