using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour {
    private Sprite[] _diceSides;
    private Image _die1Image;
    private Image _die2Image;
    public GameObject die1;
    public GameObject die2;

    private void Start()
    {
        _die1Image = die1.GetComponent<Image>();
        _die2Image = die2.GetComponent<Image>();
        _diceSides = Resources.LoadAll<Sprite>("DiceSides/");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            APIWrapper.Instance.PlayerRollDice((response, error) =>
            {
                if (error != null)
                {
                    Debug.Log(error); // TODO: Show error to player
                }
            });
        }
    }
    
    public IEnumerator RollTheDice(int[] dice)
    {
        die1.SetActive(true);
        die2.SetActive(true);

        for (var i = 0; i <= 20; i++)
        {
            var randomDie1Side = Random.Range(0, 5);
            _die1Image.sprite = _diceSides[randomDie1Side];
            
            var randomDie2Side = Random.Range(0, 5);
            _die2Image.sprite = _diceSides[randomDie2Side];

            yield return new WaitForSeconds(0.05f);
        }
        
        _die1Image.sprite = _diceSides[dice[0] - 1];
        _die2Image.sprite = _diceSides[dice[1] - 1];
        
        yield return new WaitForSeconds(1f);
        
        die1.SetActive(false);
        die2.SetActive(false);
    }
}
