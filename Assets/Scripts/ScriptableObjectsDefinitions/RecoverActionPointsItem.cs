using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Item/RecoverAP", order = 0)]
public class RecoverActionPointsItem : Item
{
    private void Awake()
    {
        appliesOn = FighterType.Player;
    }

    public override void Effect(Fighter target)
    {
        Fighter stats = target;

        target.SetActionPoints(stats.GetActionPoints() + 30);
    }
}
