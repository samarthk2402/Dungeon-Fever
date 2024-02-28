using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageGameStat : MonoBehaviour
{

    public GameObject player;
    public GameObject enemy;
    public GameObject healthText;

    private GameObject playerHealthText;
    private GameObject enemyHealthText;

    [SerializeField] private Canvas canvas;

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

        enemyHealthText = Instantiate(healthText, enemy.transform.position + new Vector3(2, 0, 0), Quaternion.identity);

        enemyHealthText.transform.SetParent(canvas.transform, false);

        playerHealthText = Instantiate(healthText, player.transform.position + new Vector3(-2, 0, 0), Quaternion.identity);

        playerHealthText.transform.SetParent(canvas.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<Attack>().health <= 0){
            playerHealthText.GetComponent<TMP_Text>().text = "0";
        }else{
            playerHealthText.GetComponent<TMP_Text>().text = player.GetComponent<Attack>().health.ToString();
        }

        if(enemy.GetComponent<Attack>().health <= 0){
            enemyHealthText.GetComponent<TMP_Text>().text = "0";
        }else{
            enemyHealthText.GetComponent<TMP_Text>().text = enemy.GetComponent<Attack>().health.ToString();
        }

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
