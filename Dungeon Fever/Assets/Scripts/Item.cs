using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemScriptableObject", order = 0)]
public class Item : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public int level;
}
