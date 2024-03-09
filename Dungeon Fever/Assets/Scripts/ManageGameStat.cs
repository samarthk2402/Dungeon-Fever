using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ManageGameStat : MonoBehaviour
{
    public List<Gradient> rarityColours = new List<Gradient>();
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
    public GameObject levelText;
    public GameObject stageText;

    private GameObject playerHealthText;
    private GameObject enemyHealthText; 
    private GameObject playerEnergyText;
    private GameObject playerLevelText;

    private bool enemyInstantiating;
    public int stage;
    public int goldToDrop;
    private int gold;
    public TMP_Text goldText;
    public RectTransform goldIcon;
    public GameObject goldCoinPrefab;
    public GameObject xpPrefab;

    [SerializeField] private Canvas canvas;

    public enum State{
        Player,
        Enemy
    }

    public State gameState;
    // Start is called before the first frame update
    void Start()
    {
        gold = 0;

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

        playerLevelText = Instantiate(levelText, player.transform.position + new Vector3(-1.5f, -1.5f, 0), Quaternion.identity);
        playerLevelText.transform.SetParent(canvas.transform, false);

    }

    // Update is called once per frame
    void Update()
    {
        goldText.text = gold.ToString();
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

            if(playerLevelText.gameObject != null){
                playerLevelText.GetComponent<TMP_Text>().text = "Lvl "+player.GetComponent<Attack>().level.ToString();
                playerLevelText.GetComponentInChildren<XPBar>().SetXP(player.GetComponent<Attack>().xp);

                if(player.GetComponent<Attack>().dead){
                    Destroy(playerLevelText);
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
        item.GetComponent<Rigidbody2D>().AddForce((player.transform.position-spawnTransform.position)*2, ForceMode2D.Impulse);

        int randColour = Random.Range(0, rarityColours.Count);
        item.GetComponentInChildren<LineRenderer>().startColor = rarityColours[randColour].Evaluate(0);
        item.GetComponentInChildren<LineRenderer>().endColor = rarityColours[randColour].Evaluate(1);

        var mainModule = item.GetComponentInChildren<ParticleSystem>().colorOverLifetime;
        mainModule.color = new ParticleSystem.MinMaxGradient(rarityColours[randColour]);

        for(int i = 0; i<goldToDrop; i++){
            //Debug.Log(i);
            Invoke("InstantiateCoin", i*0.1f);
        }

        yield return new WaitForSeconds(1);

        for(int i = 0; i<10; i++){
            //Debug.Log(i);
            Invoke("InstantiateXPSoul", i*0.05f);
        }

        yield return new WaitForSeconds(2.5f);
        Destroy(item, 0.5f);
        transitionAnim.SetTrigger("newStage");
    }

    void InstantiateCoin(){
        //Debug.Log("Instantiating coin");
        GameObject coin = Instantiate(goldCoinPrefab, spawnTransform.position, Quaternion.identity);
        StartCoroutine(LerpObject(coin.transform, goldIcon.position, 1f, true));
        Destroy(coin, 1.1f);
    }

    void InstantiateXPSoul(){
        //Debug.Log("Instantiating coin");
        GameObject soul = Instantiate(xpPrefab, spawnTransform.position, Quaternion.identity);
        StartCoroutine(LerpObject(soul.transform, playerLevelText.transform.position, 1f, false));
        Destroy(soul, 1.1f);
    }

    IEnumerator LerpObject(Transform startTransform, Vector3 endingPosition, float lerpDuration, bool isCoin)
    {
        float elapsedTime = 0f;

        Vector3 startingPosition = startTransform.position;

        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;
            startTransform.position = Vector3.Lerp(startingPosition, endingPosition, t);
            elapsedTime += Time.deltaTime;
            //Debug.Log(startTransform.position);
            yield return null;
        }

        // Ensure final position
        startTransform.position = endingPosition;
        if(isCoin){
            gold += 1;
        }else{
            player.GetComponent<Attack>().xp += 1;
        }
    }
}
