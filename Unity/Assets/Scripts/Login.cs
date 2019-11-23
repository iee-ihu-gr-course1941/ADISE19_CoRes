using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour
{
    public TextInput username;
    public TextInput password;

    public void onClick()
    {
        Debug.Log(username.Value);
        Debug.Log(password.Value);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
