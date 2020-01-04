using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Property")]
public class PropertyCard : ScriptableObject
{
    public new string name;
    public int location;
    public int price;
    public int[] rents;
    public int mortgage;
    
    public Sprite artwork;
}
