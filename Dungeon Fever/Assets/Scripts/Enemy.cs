using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/EnemyScriptableObject", order = 1)]
public class Enemy : ScriptableObject
{
    public new string name;
    public int damage;
    public int health;
    public string special;

    public AnimatorOverrideController animatorController;
}
