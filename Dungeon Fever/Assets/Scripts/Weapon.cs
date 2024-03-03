using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 0)]
public class Weapon : ScriptableObject {
    public int damage;
    public AnimatorOverrideController animatorController;
}
