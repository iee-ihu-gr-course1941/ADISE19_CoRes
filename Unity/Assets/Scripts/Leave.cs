using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leave : MonoBehaviour
{
    public SocketIo socketIo;
    
    public void OnClick()
    {
        socketIo.Close();
        
        APIWrapper.Instance.GameLeave((response, error) =>
        {
            if (error == null)
            {
                SceneManager.LoadScene("Home", LoadSceneMode.Single);
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
