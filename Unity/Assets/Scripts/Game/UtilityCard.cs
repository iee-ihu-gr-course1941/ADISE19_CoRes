using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityCard
{
    public string name;
    public int location;
    public int[] rents;
    public int mortgage;

    public UtilityCard(string name, int location, int[] rents, int mortgage)
    {
        this.name = name;
        this.location = location;
        this.rents = rents;
        this.mortgage = mortgage;
    }
}
