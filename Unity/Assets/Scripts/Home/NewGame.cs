using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGame : MonoBehaviour
{
    public GameObject dialog;
    private Button _button;
    public GameObject errorText;
    
    private void Awake()
    {
        ClearErrorText();
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate
        {
            dialog.SetActive(true);
        });
    }
    
    private void ClearErrorText()
    {
        errorText.transform.GetComponent<TextMeshProUGUI>().text = "";
    }
}
