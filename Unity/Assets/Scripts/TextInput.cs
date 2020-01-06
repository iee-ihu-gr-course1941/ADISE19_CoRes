using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TextInput : MonoBehaviour
{
    private TMP_InputField _inputField;
    private Outline _outline;
    
    public string Value
    {
        get => _inputField.text;
        set => _inputField.text = value;
    }

    public bool Highlight
    {
        get => _outline.enabled;
        set => _outline.enabled = value;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _inputField = GetComponentInChildren<TMP_InputField>();
        _outline = GetComponentInChildren<Outline>();
    }
}
