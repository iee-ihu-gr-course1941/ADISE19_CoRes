using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextInput : MonoBehaviour
{
    private InputField _inputField;
    
    public string Value
    {
        get => _inputField.text;
        set => _inputField.text = value;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _inputField = GetComponentInChildren<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
