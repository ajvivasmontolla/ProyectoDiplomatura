using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] public string itemName;
    [SerializeField] public string itemDescription;
    [SerializeField] public int actionPointsRequired;
    [SerializeField] public int itemQuantity;

    //if applies on your own team this variable is assigned "Player" if applies on enemy team, "Enemy"
    public FighterType appliesOn = FighterType.None;

    public abstract void Effect(Fighter target);
}

