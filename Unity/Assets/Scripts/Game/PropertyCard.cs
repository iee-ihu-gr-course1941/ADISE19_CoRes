using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCard
{
    public string name;
    public string color;
    public int location;
    public int[] rents;
    public int houseCost;
    public int mortgage;

    public PropertyCard(string name, string color, int location, int[] rents, int houseCost, int mortgage)
    {
        this.name = name;
        this.color = color;
        this.location = location;
        this.rents = rents;
        this.houseCost = houseCost;
        this.mortgage = mortgage;
    }
}
