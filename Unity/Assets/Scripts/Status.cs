using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    public string Value
    {
        get => _text.text;
        set => _text.text = value;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
