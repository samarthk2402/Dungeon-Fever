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
    public GameObject characterPrefab;
    public Vector3 prevEnemyPos;

    public List<Sprite> items = new List<Sprite>();
    public GameObject itemPrefab;

    public Animator transitionAnim;

    public GameObject healthText;
    public GameObject energyText;
    public GameObject levelText;
    public GameObject stageText;

    public int stage;
    public int goldToDrop;
    private int gold;
    public TMP_Text goldText;
    public GameObject goldIcon;
    public GameObject goldCoinPrefab;
    public GameObject xpPrefab;
    public int xpToDrop;
    public int rareEnemyChance;
    public int superRareEnemyChance;

    private bool prevRareEnemy;
    private bool prevSuperRareEnemy;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform goldCounter;

    public List<Vector3> enemyPositions = new List<Vector3>();
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> enemyHealthTexts = new List<GameObject>();
    public int currentEnemyIndex;  
    private GameObject currEnemy;

    public List<GameObject> characters = new List<GameObject>();
    public int currentCharIndex;
    public GameObject currChar;

    List<List<GameObject>> charUIElements = new List<List<GameObject>>();

    public GameObject attackButton;
    public GameObject abilityButton;

    private Vector3 goldDestination;
    private bool lootFinished = true;

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
        currChar.GetComponent<Attack>().turn = true;

        foreach(Vector3 pos in enemyPositions){
            InstantiateEnemy(pos);
        }

        // foreach(GameObject enemy in enemies){
        //     InstantiateEnemyHealth(enemy);
        // }

        foreach(GameObject character in characters){
            GameObject playerHealthText = Instantiate(healthText, character.transform.position + new Vector3(-0.8f, 0.3f, 0), Quaternion.identity);
            playerHealthText.transform.SetParent(canvas.transform, false);

            GameObject playerEnergyText = Instantiate(energyText, character.transform.position + new Vector3(-0.8f, 0f, 0), Quaternion.identity);
            playerEnergyText.transform.SetParent(canvas.transform, false);

            GameObject playerLevelText = Instantiate(levelText, character.transform.position + new Vector3(-1.2f, -1f, 0), Quaternion.identity);
            playerLevelText.transform.SetParent(canvas.transform, false);

            charUIElements.Add(new List<GameObject> {playerHealthText, playerEnergyText, playerLevelText});
        }

        // Get the position of the UI element in screen space
        Vector3 screenPosition = goldCounter.position;

        // Convert screen space position to world space
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        goldDestination = worldPosition;
        //Debug.Log("World space position of UI element: " + worldPosition);
        //Debug.Log("Local pos: " + goldIcon.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        attackButton.GetComponent<AttackButton>().player = currChar;
        abilityButton.GetComponent<AttackButton>().player = currChar;
        goldText.text = gold.ToString();
        stageText.GetComponent<TMP_Text>().text = "Stage: "+stage.ToString();

        if(enemies!= null){
            // Check if any enemy in the list is destroyed
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].GetComponent<EnemyAttack>().dead)
                {
                    // Remove the destroyed GameObject from the list
                    prevEnemyPos = enemies[i].transform.position;
                    if(enemies[i].GetComponent<EnemyAttack>().superRare){
                        prevSuperRareEnemy = true;
                        prevRareEnemy = false;
                    }else if(enemies[i].GetComponent<EnemyAttack>().rare){
                        prevRareEnemy = true;
                        prevSuperRareEnemy = false;
                    }else{
                        prevRareEnemy = false;
                        prevSuperRareEnemy = false;
                    }
                    Destroy(enemies[i].gameObject);
                    StartCoroutine(ItemDrop());
                    enemies.RemoveAt(i);
                    Destroy(enemyHealthTexts[i]);
                    enemyHealthTexts.RemoveAt(i);
                }
            }
        }

        if(characters != null){
            // Check if any enemy in the list is destroyed
            for (int i = characters.Count - 1; i >= 0; i--)
            {
                if (characters[i]==null)
                {
                    characters.RemoveAt(i);
                    foreach(GameObject ui in charUIElements[i]){
                        Destroy(ui);
                    }
                    charUIElements.RemoveAt(i);
                }
            }
        }

        //Debug.Log(enemies.Count);
        for(int i=0; i<characters.Count; i++){
            GameObject playerHealthText = charUIElements[i][0];
            GameObject playerEnergyText = charUIElements[i][1];
            GameObject playerLevelText = charUIElements[i][2];
            if(characters[i].gameObject != null){
                if(playerHealthText.gameObject != null){
                    if(characters[i].GetComponent<Attack>().health <= 0){
                        playerHealthText.GetComponent<TMP_Text>().text = "0";
                    }else{
                        playerHealthText.GetComponent<TMP_Text>().text = characters[i].GetComponent<Attack>().health.ToString();
                    }

                    if(characters[i].GetComponent<Attack>().dead){
                        Destroy(playerHealthText);
                        timer.StopTimer();
                    }
                }

                if(playerEnergyText.gameObject != null){
                    if(characters[i].GetComponent<Attack>().energy <= 0){
                        playerEnergyText.GetComponent<TMP_Text>().text = "0";
                    }else{
                        playerEnergyText.GetComponent<TMP_Text>().text = characters[i].GetComponent<Attack>().energy.ToString();
                    }

                    if(characters[i].GetComponent<Attack>().dead){
                        Destroy(playerEnergyText);
                    }

                    // playerHealthText.transform.position = player.transform.position + new Vector3(-2, 0, 0);
                }

                if(playerLevelText.gameObject != null){
                    playerLevelText.GetComponent<TMP_Text>().text = "Lvl "+characters[i].GetComponent<Attack>().level.ToString();
                    playerLevelText.GetComponentInChildren<XPBar>().SetXP(characters[i].GetComponent<Attack>().xp);

                    if(characters[i].GetComponent<Attack>().dead){
                        Destroy(playerLevelText);
                    }

                    // playerHealthText.transform.position = player.transform.position + new Vector3(-2, 0, 0);
                }
            }
        }

        if(enemies != null && enemyHealthTexts != null){
            for(int i=0; i<enemies.Count; i++){
                if(enemies[i].gameObject != null){
                    if(enemies[i].GetComponent<EnemyAttack>().health <= 0){
                        enemyHealthTexts[i].GetComponent<TMP_Text>().text = "0";
                    }else{
                        enemyHealthTexts[i].GetComponent<TMP_Text>().text = enemies[i].GetComponent<EnemyAttack>().health.ToString();
                    }
                }
            }
        }

        switch(gameState){
            case State.Player:
                if(characters.Count>0){
                    currChar = characters[currentCharIndex];
                }
                if(lootFinished && characters!= null){
                    if(currChar.gameObject != null){
                        if(currChar.GetComponent<Attack>().turnCompleted){
                            currChar.GetComponent<Attack>().turn = false;
                            currChar.GetComponent<Attack>().turnCompleted = false;

                            currentCharIndex += 1;
                            if(currentCharIndex>=characters.Count || enemies.Count<=0){
                                if(currentEnemyIndex < enemies.Count){
                                    currEnemy = enemies[currentEnemyIndex];
                                    currEnemy.GetComponent<EnemyAttack>().characters = characters;
                                    currEnemy.GetComponent<EnemyAttack>().turn = true;
                                    currentCharIndex = 0;
                                    gameState = State.Enemy;
                                }else{
                                    if(stage>=9){
                                        SceneManager.LoadScene(2);
                                    }else{
                                        currentCharIndex = 0;
                                        StartCoroutine(NewStage());
                                    }
                                }
                            }
                        
                        }else{
                            //Debug.Log(currChar);
                            if(!currChar.GetComponent<Attack>().clicked){
                                currChar.GetComponent<Attack>().turn = true;
                            }
                            
                        }
                    }
                }else{
                    currChar.GetComponent<Attack>().turn = false;
                }
                
                for(int i=0; i<characters.Count; i++){
                    if(i>currentCharIndex){
                        characters[i].GetComponent<Attack>().clicked = false;
                    }
                }
                // if(!enemyInstantiating && enemy==null){
                //     StartCoroutine(ItemDrop());
                //     enemyInstantiating = true;
                // }
                break;
            case State.Enemy:
                //Debug.Log("Enemy turn!");
                currEnemy = enemies[currentEnemyIndex];

                if(currEnemy.gameObject != null){
                    //Debug.Log(currEnemy.GetComponent<EnemyAttack>().enemy);
                    if(currEnemy.GetComponent<EnemyAttack>().turnCompleted){
                        currEnemy.GetComponent<EnemyAttack>().turn = false;
                        //currEnemy.GetComponent<EnemyAttack>().turnCompleted = false;
                        if(lootFinished){
                            currentEnemyIndex += 1;
                            if(currentEnemyIndex>=enemies.Count){
                                currChar = characters[currentCharIndex];
                                currChar.GetComponent<Attack>().turn = true;
                                foreach(GameObject character in characters){
                                    character.GetComponent<Attack>().option = Attack.Option.Attack;
                                    character.GetComponent<Attack>().clicked = false;
                                }
                                currentEnemyIndex = 0;
                                gameState = State.Player;
                            }
                        }
                    }else{
                        if(lootFinished){
                            currEnemy.GetComponent<EnemyAttack>().characters = characters;

                            currEnemy.GetComponent<EnemyAttack>().turn = true;
                        }
                    }
                }
                break;
        }
    }

    public void InstantiateEnemy(Vector3 pos){
        int rand = Random.Range(0, enemyTypes.Count);
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemies.Add(enemy);
        enemy.GetComponent<EnemyAttack>().canvas = canvas;
        enemy.GetComponent<EnemyAttack>().enemy = enemyTypes[rand];

        rand = Random.Range(0, 100);     

        if(rand<=superRareEnemyChance){
            enemy.GetComponent<EnemyAttack>().superRare = true;

        }else if(rand<=rareEnemyChance){
            enemy.GetComponent<EnemyAttack>().rare = true;

        }else{
            enemy.GetComponent<EnemyAttack>().superRare = false;
            enemy.GetComponent<EnemyAttack>().rare = false;
            prevRareEnemy = false;

        }
        
        InstantiateEnemyHealth(enemy);

        currChar.GetComponent<Attack>().turn = true;
        currChar.GetComponent<Attack>().option = Attack.Option.Attack;
        //stage += 1;
    }

    IEnumerator ItemDrop(){
        lootFinished = false;
        GameObject item = Instantiate(itemPrefab, prevEnemyPos, Quaternion.identity);
        int rand = Random.Range(0, items.Count);
        item.GetComponentInChildren<SpriteRenderer>().sprite = items[rand];
        item.GetComponent<Rigidbody2D>().AddForce((currChar.transform.position-prevEnemyPos)*1.5f, ForceMode2D.Impulse);

        int randColour;
        if(prevSuperRareEnemy){
            randColour = Random.Range(rarityColours.Count-2, rarityColours.Count);
            goldToDrop = 15;
            xpToDrop = 30;
        }else if(prevRareEnemy){
            randColour = Random.Range(rarityColours.Count-3, rarityColours.Count);
            goldToDrop = 10;
            xpToDrop = 20;
        }else{
            randColour = Random.Range(0, rarityColours.Count-3);
            goldToDrop = 5;
            xpToDrop = 10;
        }


        item.GetComponentInChildren<LineRenderer>().startColor = rarityColours[randColour].Evaluate(0);
        item.GetComponentInChildren<LineRenderer>().endColor = rarityColours[randColour].Evaluate(1);

        Material material = item.GetComponentInChildren<SpriteRenderer>().material;
        //Debug.Log(material);

        // Set the outline color property of the material to the new color
        material.SetColor("_color", rarityColours[randColour].Evaluate(0)*randColour);

        var mainModule = item.GetComponentInChildren<ParticleSystem>().colorOverLifetime;
        mainModule.color = new ParticleSystem.MinMaxGradient(rarityColours[randColour]);

        for(int i = 0; i<goldToDrop; i++){
            //Debug.Log(i);
            Invoke("InstantiateCoin", i*0.5f/goldToDrop);
        }

        yield return new WaitForSeconds(1);

        for(int i = 0; i<xpToDrop; i++){
            //Debug.Log(i);
            Invoke("InstantiateXPSoul", i*0.5f/xpToDrop);
        }

        yield return new WaitForSeconds(2f);
        lootFinished = true;
        Destroy(item);
    }

    IEnumerator NewStage(){
        yield return new WaitForSeconds(1f);
        transitionAnim.SetTrigger("newStage");
        stage += 1;
    }

    void InstantiateCoin(){
        //Debug.Log("Instantiating coin");
        GameObject coin = Instantiate(goldCoinPrefab, prevEnemyPos, Quaternion.identity);
        StartCoroutine(LerpObject(coin.transform, goldDestination, 1f, true));
        Destroy(coin, 1.1f);
    }

    void InstantiateXPSoul(){
        //Debug.Log("Instantiating coin");
        GameObject soul = Instantiate(xpPrefab, prevEnemyPos, Quaternion.identity);
        StartCoroutine(LerpObject(soul.transform, charUIElements[currentCharIndex][2].transform.position, 0.8f, false));
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
            goldIcon.GetComponent<Animator>().SetTrigger("moreGold");
        }else{
            currChar.GetComponent<Attack>().xp += 1;
        }
    }

    void InstantiateEnemyHealth(GameObject enemy){
        GameObject enemyHealthText = Instantiate(healthText, enemy.transform.position + new Vector3(1.5f, 0f, 0), Quaternion.identity);
        enemyHealthText.transform.SetParent(canvas.transform, false);
        enemyHealthTexts.Add(enemyHealthText);
    }
}
