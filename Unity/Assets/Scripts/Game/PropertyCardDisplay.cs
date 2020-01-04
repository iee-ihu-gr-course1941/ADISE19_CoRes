using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyCardDisplay : MonoBehaviour
{
    public PropertyCard card;

    public Text nameText;
    public Text[] rentText = new Text[6];
    public Text bookCostText;
    public Text degreeCostText;
    public Text mortgageText;

    public Image artworkImage;
    
    // Start is called before the first frame update
    void Start()
    {
        nameText.text = card.name;
        
        for (int i = 0; i < 6; i++)
        {
            rentText[i].text = card.rents[i].ToString();
        }

        bookCostText.text = card.mortgage.ToString(); // TODO: Change
        degreeCostText.text = card.mortgage.ToString();
        mortgageText.text = card.mortgage.ToString();

        artworkImage.sprite = card.artwork;
    }
}
