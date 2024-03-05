using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ManageGameStat : MonoBehaviour
{
    public Timer timer;
    public List<Enemy> enemyTypes = new List<Enemy>();
    public GameObject enemyPrefab;
    public Transform spawnTransform;

    public List<Sprite> items = new List<Sprite>();
    public GameObject itemPrefab;

    public Animator transitionAnim;

    public GameObject player;
    public GameObject enemy;
    public GameObject healthText;
    public GameObject energyText;
    public GameObject stageText;

    private GameObject playerHealthText;
    private GameObject enemyHealthText; 
    private GameObject playerEnergyText;
    private bool enemyInstantiating;
    public int stage;

    [SerializeField] private Canvas canvas;

    public enum State{
        Player,
        Enemy
    }

    public State gameState;
    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponentInChildren<Timer>();
        timer.StartTimer();
        stage = 0;

        gameState = State.Player;
        player.GetComponent<Attack>().turn = true;

        InstantiateEnemy();

        enemyHealthText = Instantiate(healthText, enemy.transform.position + new Vector3(2, 0f, 0), Quaternion.identity);
        enemyHealthText.transform.SetParent(canvas.transform, false);

        playerHealthText = Instantiate(healthText, player.transform.position + new Vector3(-1, 0.3f, 0), Quaternion.identity);
        playerHealthText.transform.SetParent(canvas.transform, false);

        playerEnergyText = Instantiate(energyText, player.transform.position + new Vector3(-1, -0.3f, 0), Quaternion.identity);
        playerEnergyText.transform.SetParent(canvas.transform, false);

    }

    // Update is called once per frame
    void Update()
    {
        stageText.GetComponent<TMP_Text>().text = "Stage: "+stage.ToString();
        if(player.gameObject != null){
            if(playerHealthText.gameObject != null){
                if(player.GetComponent<Attack>().health <= 0){
                    playerHealthText.GetComponent<TMP_Text>().text = "0";
                }else{
                    playerHealthText.GetComponent<TMP_Text>().text = player.GetComponent<Attack>().health.ToString();
                }

                if(player.GetComponent<Attack>().dead){
                    Destroy(playerHealthText);
                    timer.StopTimer();
                }

                // playerHealthText.transform.position = player.transform.position + new Vector3(-2, 0, 0);
            }

            if(playerEnergyText.gameObject != null){
                if(player.GetComponent<Attack>().health <= 0){
                    playerEnergyText.GetComponent<TMP_Text>().text = "0";
                }else{
                    playerEnergyText.GetComponent<TMP_Text>().text = player.GetComponent<Attack>().energy.ToString();
                }

                if(player.GetComponent<Attack>().dead){
                    Destroy(playerEnergyText);
                }

                // playerHealthText.transform.position = player.transform.position + new Vector3(-2, 0, 0);
            }
        }

        if(enemy.gameObject != null){
            if(enemy.GetComponent<EnemyAttack>().health <= 0){
            enemyHealthText.GetComponent<TMP_Text>().text = "0";
            }else{
                enemyHealthText.GetComponent<TMP_Text>().text = enemy.GetComponent<EnemyAttack>().health.ToString();
            }
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
                        }else{
                            if(stage>=9){
                                SceneManager.LoadScene(2);
                            }
                        }
                    }
                }

                if(!enemyInstantiating && enemy==null){
                    StartCoroutine(ItemDrop());
                    enemyInstantiating = true;
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
                        player.GetComponent<Attack>().option = Attack.Option.Attack;
                        gameState = State.Player;
                    }
                }
                break;
        }
    }

    public void InstantiateEnemy(){
        int rand = Random.Range(0, enemyTypes.Count);
        enemy = Instantiate(enemyPrefab, spawnTransform.position, Quaternion.identity);
        enemy.GetComponent<EnemyAttack>().player = player.gameObject;
        enemy.GetComponent<EnemyAttack>().canvas = canvas;
        enemy.GetComponent<EnemyAttack>().enemy = enemyTypes[rand];
        player.GetComponent<Attack>().turn = true;
        player.GetComponent<Attack>().option = Attack.Option.Attack;
        enemyInstantiating = false;
        stage += 1;
    }

    IEnumerator ItemDrop(){
        GameObject item = Instantiate(itemPrefab, spawnTransform.position, Quaternion.identity);
        int rand = Random.Range(0, items.Count);
        item.GetComponentInChildren<SpriteRenderer>().sprite = items[rand];
        yield return new WaitForSeconds(1);
        Destroy(item);
        transitionAnim.SetTrigger("newStage");
    }

    // IEnumerator InstantiateEnemyAfterDelay(float delay){
    //     if(enemy.gameObject == null){
    //         transitionAnim.SetTrigger("newStage");
    //     }
    //     yield return new WaitForSeconds(delay);
    //     if(enemy.gameObject == null){
    //         InstantiateEnemy();
    //     }
    // }
}
