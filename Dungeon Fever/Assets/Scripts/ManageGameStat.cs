using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageGameStat : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public enum State{
        Player,
        Enemy
    }

    public State gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = State.Player;
        player.GetComponent<Attack>().turn = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch(gameState){
            case State.Player:
                if(player.GetComponent<Attack>().turnCompleted){
                    player.GetComponent<Attack>().turn = false;
                    player.GetComponent<Attack>().turnCompleted = false;
                    enemy.GetComponent<Attack>().turn = true;
                    gameState = State.Enemy;
                }
                break;
            case State.Enemy:
                //Debug.Log("Enemy turn!");

                if(enemy.GetComponent<Attack>().turnCompleted){
                    //Debug.Log("Enemy turn completed");
                    enemy.GetComponent<Attack>().turn = false;
                    enemy.GetComponent<Attack>().turnCompleted = false;
                    player.GetComponent<Attack>().turn = true;
                    gameState = State.Player;
                }
                break;
        }
    }
}
