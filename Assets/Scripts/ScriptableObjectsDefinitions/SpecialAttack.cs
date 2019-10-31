using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "SpAtk_", menuName = "Special Attack", order = 1)]
public class SpecialAttack : ScriptableObject
{
    [SerializeField] public string spAttackName;
    [SerializeField] public string spAttackDescription;
    [SerializeField] public int actionPointsRequired;

    [SerializeField] public int attackPower;
    [SerializeField] public int damagingTurnsCant;
}
