using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Leave : MonoBehaviour
{
    public SocketIo socketIo;
    
    public void OnClick()
    {
        APIWrapper.Instance.GameLeave((response, error) =>
        {
            SceneManager.LoadScene("Home", LoadSceneMode.Single);

            if (error != null && response["error"] == null)
            {
                throw new Exception(error);
            }
        });
        
        socketIo.Close();
    }
}
