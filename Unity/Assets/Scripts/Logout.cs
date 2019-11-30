using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logout : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnClick()
    {
        AuthenticationManager.Instance.Logout();
    }
}
