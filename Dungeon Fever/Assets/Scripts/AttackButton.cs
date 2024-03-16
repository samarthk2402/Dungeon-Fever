using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    public void Attack()
    {
        player.GetComponent<Attack>().SetAttack();
    }

    // Update is called once per frame
    public void Ability()
    {
        player.GetComponent<Attack>().SetAbility();
    }
}
