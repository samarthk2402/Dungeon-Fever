using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/CharacterScriptableObject", order = 0)]
public class Character : ScriptableObject {
    public new string name;
    public int strength;
    public int health;
    public int energy;
    public int abilityDamage;
    public int abilityCost;
    public GameObject abilityEffect;
    public AnimatorOverrideController animatorController;
}

