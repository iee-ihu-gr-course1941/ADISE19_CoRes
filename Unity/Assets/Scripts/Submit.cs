using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Submit : MonoBehaviour
{
    private Button _button;

    public event EventHandler Click;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate {
            EventHandler handler = Click;
            handler?.Invoke(_button, null);
        });
    }
}
