using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationCard
{
    public string name;
    public int location;
    public int[] rents;
    public int mortgage;

    public StationCard(string name, int location, int[] rents, int mortgage)
    {
        this.name = name;
        this.location = location;
        this.rents = rents;
        this.mortgage = mortgage;
    }
}
