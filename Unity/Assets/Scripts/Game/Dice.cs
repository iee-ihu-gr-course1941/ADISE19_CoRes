using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour {
    private Sprite[] _diceSides;
    private Image _image;

    public int desiredRoll;

    private void Start () {
        _image = GetComponent<Image>();

        _diceSides = Resources.LoadAll<Sprite>("DiceSides/");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("RollTheDice");
        }
    }
    
    private IEnumerator RollTheDice()
    {
        for (int i = 0; i <= 20; i++)
        {
            var randomDiceSide = Random.Range(0, 5);
            _image.sprite = _diceSides[randomDiceSide];

            yield return new WaitForSeconds(0.05f);
        }
        
        _image.sprite = _diceSides[desiredRoll - 1];
    }
}
