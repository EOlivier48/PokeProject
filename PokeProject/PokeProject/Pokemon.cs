using System;
using System.Collections.Generic;

public class pokemon
{
    private string name;
    private string[] types = new string[2];
    private Dictionary<string, double> damageRelationships = new Dictionary<string, double>();

    public pokemon(string name = "MissingNo.")
    {
        this.name = name;
    }

    /* Takes a given type/multiplier and adds it to the existing damage relationships appropriately 
     */
    public void AddDamageRelationship(string name, double multiplier)
    {
        try
        {
            double existing = this.damageRelationships[name];
            double newMultiplier = existing * multiplier;
            if (newMultiplier == 1)
            {
                this.damageRelationships.Remove(name);
            }
            else
            {
                this.damageRelationships[name] = newMultiplier;
            }
        }
        catch (KeyNotFoundException)
        {
            this.damageRelationships.Add(name, multiplier);
        }
        return;
    }

    public void SetType(int slot, string typeName)
    {
        types[slot] = typeName;
        return;
    }

    public Dictionary<string,double> GetDamageRelationships()
    {
        return damageRelationships;
    }

    public string[] GetTypes()
    {
        return types;
    }

    public string GetName()
    {
        return name;
    }
}