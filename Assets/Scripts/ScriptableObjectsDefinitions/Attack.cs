using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Atk_", menuName = "Attack", order = 0)]
public class Attack : ScriptableObject
{
    [SerializeField] public string attackName;
    [SerializeField] public string attackDescription;

    [SerializeField] public int attackPower;
}
