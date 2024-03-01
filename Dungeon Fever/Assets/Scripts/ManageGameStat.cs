using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageGameStat : MonoBehaviour
{
    public List<Enemy> enemyTypes = new List<Enemy>();
    public GameObject enemyPrefab;
    public Transform spawnTransform;

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

        InstantiateEnemy();

        enemyHealthText = Instantiate(healthText, enemy.transform.position + new Vector3(2, 0, 0), Quaternion.identity);

        enemyHealthText.transform.SetParent(canvas.transform, false);

        playerHealthText = Instantiate(healthText, player.transform.position + new Vector3(-2, 0, 0), Quaternion.identity);

        playerHealthText.transform.SetParent(canvas.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(player.gameObject != null){
            if(player.GetComponent<Attack>().health <= 0){
            playerHealthText.GetComponent<TMP_Text>().text = "0";
            }else{
                playerHealthText.GetComponent<TMP_Text>().text = player.GetComponent<Attack>().health.ToString();
            }
        }

        if(enemy.gameObject != null){
            if(enemy.GetComponent<EnemyAttack>().health <= 0){
            enemyHealthText.GetComponent<TMP_Text>().text = "0";
            }else{
                enemyHealthText.GetComponent<TMP_Text>().text = enemy.GetComponent<EnemyAttack>().health.ToString();
            }
        }else{
            StartCoroutine(InstantiateEnemyAfterDelay(1.5f));
        }

        switch(gameState){
            case State.Player:
                if(player.gameObject != null){
                    if(player.GetComponent<Attack>().turnCompleted){
                        player.GetComponent<Attack>().turn = false;
                        player.GetComponent<Attack>().turnCompleted = false;
                        if(enemy != null){
                            enemy.GetComponent<EnemyAttack>().turn = true;
                            gameState = State.Enemy;
                        }
                    }
                }
                break;
            case State.Enemy:
                //Debug.Log("Enemy turn!");
                if(enemy.gameObject != null){
                    if(enemy.GetComponent<EnemyAttack>().turnCompleted){
                        //Debug.Log("Enemy turn completed");
                        enemy.GetComponent<EnemyAttack>().turn = false;
                        enemy.GetComponent<EnemyAttack>().turnCompleted = false;
                        player.GetComponent<Attack>().turn = true;
                        gameState = State.Player;
                    }
                }
                break;
        }
    }

    void InstantiateEnemy(){
        int rand = Random.Range(0, enemyTypes.Count);
        enemy = Instantiate(enemyPrefab, spawnTransform.position, Quaternion.identity);
        enemy.GetComponent<EnemyAttack>().player = player.gameObject;
        enemy.GetComponent<EnemyAttack>().canvas = canvas;
        enemy.GetComponent<EnemyAttack>().enemy = enemyTypes[rand];
        player.GetComponent<Attack>().turn = true;
    }

    IEnumerator InstantiateEnemyAfterDelay(float delay){
        yield return new WaitForSeconds(delay);
        if(enemy.gameObject == null){
            InstantiateEnemy();
        }
    }
}
