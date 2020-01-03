using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logout : MonoBehaviour
{
    public void OnClick()
    {
        AuthenticationManager.Instance.Logout();
    }
}
