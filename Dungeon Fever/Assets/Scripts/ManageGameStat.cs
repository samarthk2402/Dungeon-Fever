using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManageGameStat : MonoBehaviour
{
    public List<Gradient> rarityColours = new List<Gradient>();
    public List<string> rarityNames = new List<string>();
    public Timer timer;
    public List<Enemy> enemyTypes = new List<Enemy>();
    public List<Enemy> bossTypes = new List<Enemy>();
    public GameObject enemyPrefab;
    public GameObject characterPrefab;
    public Vector3 prevEnemyPos;

    public List<Item> items = new List<Item>();
    public GameObject itemPrefab;

    public Animator transitionAnim;

    public GameObject healthText;
    public GameObject energyText;
    public GameObject levelText;
    public GameObject stageText;

    public int stage;
    public int goldToDrop;
    public int crystalsToDrop;
    private int gold;
    private int crystals;
    public TMP_Text goldText;
    public GameObject goldIcon;
    public TMP_Text crystalText;
    public GameObject crystalIcon;
    public GameObject goldCoinPrefab;
    public GameObject xpPrefab;
    public GameObject crystalPrefab;
    public int xpToDrop;
    public int rareEnemyChance;
    public int superRareEnemyChance;

    private bool prevRareEnemy;
    private bool prevSuperRareEnemy;

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform goldCounter;
    [SerializeField] private RectTransform crystalCounter;

    public List<Vector3> enemyPositions = new List<Vector3>();
    private List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> enemyHealthTexts = new List<GameObject>();
    public int currentEnemyIndex;  
    private GameObject currEnemy;

    public List<GameObject> characters = new List<GameObject>();
    public int currentCharIndex;
    public GameObject currChar;

    List<List<GameObject>> charUIElements = new List<List<GameObject>>();

    public GameObject attackButton;
    public GameObject abilityButton;
    public GameObject options;
    private OptionsManager om;

    private Vector3 goldDestination;
    private Vector3 crystalDestination;
    private Vector3 xpDestination;
    public bool lootFinished = true;

    public GameObject floor;

    public Color energyTextColor;

    public enum State{
        Player,
        Enemy
    }

    public State gameState;
    // Start is called before the first frame update
    void Start()
    {
        om = options.GetComponent<OptionsManager>();
        attackButton.SetActive(false);
        abilityButton.SetActive(false);
        gold = 0;

        timer = GetComponentInChildren<Timer>();
        timer.StartTimer();
        stage = 1;

        gameState = State.Player;
        currChar.GetComponent<Attack>().turn = true;

        InstantiateEnemy(enemyPositions[1], false);

        // foreach(Vector3 pos in enemyPositions){
        //     InstantiateEnemy(pos);
        // }

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
        Vector3 goldScreenPosition = goldCounter.position;

        // Convert screen space position to world space
        Vector3 goldWorldPosition = Camera.main.ScreenToWorldPoint(goldScreenPosition);
        goldDestination = goldWorldPosition;

        // Get the position of the UI element in screen space
        Vector3 crystalScreenPosition = crystalCounter.position;

        // Convert screen space position to world space
        Vector3 crystalWorldPosition = Camera.main.ScreenToWorldPoint(crystalScreenPosition);
        crystalDestination = crystalWorldPosition;

        Vector3 optionsScreenPosition = options.transform.position;

        Vector3 optionsWorldPosition = Camera.main.ScreenToWorldPoint(optionsScreenPosition);
        //Debug.Log("World space position of UI element: " + goldWorldPosition);
        //Debug.Log("Local pos: " + goldIcon.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        om.player = currChar;
        attackButton.GetComponent<AttackButton>().player = currChar;
        abilityButton.GetComponent<AttackButton>().player = currChar;
        goldText.text = gold.ToString();
        crystalText.text = crystals.ToString();
        stageText.GetComponent<TMP_Text>().text = "Stage: "+stage.ToString();

        if(enemies!= null){
            // Check if any enemy in the list is destroyed
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].GetComponent<EnemyAttack>().dead)
                {
                    // Remove the destroyed GameObject from the list
                    prevEnemyPos = enemies[i].transform.position;
                    floor.transform.position = enemies[i].transform.position + new Vector3(0, -1f, 0);
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
                    Destroy(enemies[i].gameObject, 2);
                    StartCoroutine(ItemDrop());
                    enemies.RemoveAt(i);
                    Destroy(enemyHealthTexts[i]);
                    enemyHealthTexts.RemoveAt(i);
                    // if(characters.Count <= 1){
                    //     gameState = State.Enemy;
                    // }
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
                        playerHealthText.GetComponent<TMP_Text>().color = Color.white;
                    }else{
                        playerHealthText.GetComponent<TMP_Text>().text = characters[i].GetComponent<Attack>().health.ToString();
                        if(characters[i].GetComponent<Attack>().health == characters[i].GetComponent<Attack>().character.health){
                            playerHealthText.GetComponent<TMP_Text>().color = Color.green;
                        }else{
                            playerHealthText.GetComponent<TMP_Text>().color = Color.white;
                        }
                    }

                    if(characters[i].GetComponent<Attack>().dead){
                        Destroy(playerHealthText);
                        timer.StopTimer();
                    }
                }

                if(playerEnergyText.gameObject != null){
                    if(characters[i].GetComponent<Attack>().energy <= 0){
                        playerEnergyText.GetComponent<TMP_Text>().text = "0";
                        playerEnergyText.GetComponent<TMP_Text>().color = Color.white;
                    }else{
                        playerEnergyText.GetComponent<TMP_Text>().text = characters[i].GetComponent<Attack>().energy.ToString();

                        if(characters[i].GetComponent<Attack>().energy == characters[i].GetComponent<Attack>().character.energy){
                            playerEnergyText.GetComponent<TMP_Text>().color = energyTextColor;
                        }else{
                            playerEnergyText.GetComponent<TMP_Text>().color = Color.white;
                        }
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
        attackButton.SetActive(false);
        abilityButton.SetActive(false);
        switch(gameState){
            case State.Player:
                //if more than 0 players set current character to current character index
                if(characters!=null && characters.Count>0){
                    currChar = characters[currentCharIndex];
                }

                if(enemies.Count>0 && !currChar.GetComponent<Attack>().selected){
                    attackButton.SetActive(true);
                    abilityButton.SetActive(true);
                }

                if(characters!= null && lootFinished){ //wait for loot to drop and if characters exist
                    if(currChar.gameObject != null){ //check if current character alive
                        if(currChar.GetComponent<Attack>().turnCompleted){ //if turn completed
                            //Set turn to false
                            currChar.GetComponent<Attack>().turn = false;
                            currChar.GetComponent<Attack>().turnCompleted = false;
                            currChar.GetComponent<Attack>().clicked = false;
                            currChar.GetComponent<Attack>().selected = false;
                            //Move onto next character
                            currentCharIndex += 1;
                            //Check if looped through all players
                            if(currentCharIndex>=characters.Count){
                                //Check if there are any enemies left
                                if(enemies.Count>0){
                                    currEnemy = enemies[currentEnemyIndex];
                                    // currEnemy.GetComponent<EnemyAttack>().characters = characters;
                                    // currEnemy.GetComponent<EnemyAttack>().turn = true;
                                    currentCharIndex = 0;
                                    gameState = State.Enemy;
                                }else{
                                    currentCharIndex = 0;
                                    StartCoroutine(NewStage());
                                }
                            }else{
                                if(enemies.Count<=0){

                                        currentCharIndex = 0;
                                        foreach(GameObject character in characters){
                                            character.GetComponent<Attack>().selected = false;
                                        }
                                        StartCoroutine(NewStage());
                                }
                            }
                        
                        }else{ //if turn waiting to be taken...
                            if(!currChar.GetComponent<Attack>().clicked){
                                currChar.GetComponent<Attack>().turn = true;
                                options.transform.position = Camera.main.WorldToScreenPoint(currChar.transform.position + new Vector3(0, 1, 0));
                                //its is there turn until they have clicked
                            }
                            
                        }
                    }
                }else{ //if loot dropping or all characters dead
                    currChar.GetComponent<Attack>().turn = false; //pause turn
                }
                
                //iterate through all characters
                // for(int i=0; i<characters.Count; i++){
                //     //set all the 
                //     if(i>currentCharIndex){
                //         characters[i].GetComponent<Attack>().clicked = false;
                //     }
                // }

                break;
            case State.Enemy:
                if(enemies!=null){
                    currEnemy = enemies[currentEnemyIndex];
                }

                if(currEnemy.gameObject != null && lootFinished){
                    if(currEnemy.GetComponent<EnemyAttack>().turnCompleted){
                        currEnemy.GetComponent<EnemyAttack>().turn = false;
                        currEnemy.GetComponent<EnemyAttack>().turnCompleted = false;
                        currentEnemyIndex += 1;
                        if(currentEnemyIndex>=enemies.Count){
                            currChar = characters[currentCharIndex];
                            // currChar.GetComponent<Attack>().turn = true;
                            foreach(GameObject character in characters){
                                character.GetComponent<Attack>().option = Attack.Option.Attack;
                                character.GetComponent<Attack>().clicked = false;
                                character.GetComponent<Attack>().selected = false;
                            }
                            currentEnemyIndex = 0;
                            currentCharIndex = 0;
                            gameState = State.Player;
                        }
                    }else{ //waiting for turn to be completed
                        currEnemy.GetComponent<EnemyAttack>().characters = characters;
                        currEnemy.GetComponent<EnemyAttack>().turn = true;
                    }
                }
                break;
        }
    }

    public void InstantiateEnemy(Vector3 pos, bool isBoss){
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        enemies.Add(enemy);
        enemy.GetComponent<EnemyAttack>().canvas = canvas;
        if(isBoss){
            int rand = Random.Range(0, bossTypes.Count);
            enemy.GetComponent<EnemyAttack>().enemy = bossTypes[rand];
        }else{
            int rand = Random.Range(0, enemyTypes.Count);
            enemy.GetComponent<EnemyAttack>().enemy = enemyTypes[rand];
        }

        int chance = Random.Range(0, 100);     

        if(chance<=superRareEnemyChance){
            enemy.GetComponent<EnemyAttack>().superRare = true;

        }else if(chance<=rareEnemyChance){
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
        item.GetComponentInChildren<SpriteRenderer>().sprite = items[rand].sprite;
        item.GetComponent<Rigidbody2D>().AddForce((currChar.transform.position-prevEnemyPos)*2f, ForceMode2D.Impulse);

        GameObject nameText = item.transform.Find("Canvas/Name").gameObject;
        GameObject levelText = item.transform.Find("Canvas/Level").gameObject;
        GameObject rarityText = item.transform.Find("Canvas/Rarity").gameObject;

        nameText.GetComponent<TMP_Text>().text = items[rand].name;
        levelText.GetComponent<TMP_Text>().text = "Lvl "+items[rand].level.ToString();

        int randColour;
        int randNum  = Random.Range(1, 100);
        if(prevSuperRareEnemy){
            randColour = Random.Range(rarityColours.Count-2, rarityColours.Count);
            goldToDrop = 15;
            xpToDrop = 60;
            crystalsToDrop = 10;
        }else if(prevRareEnemy){
            randColour = Random.Range(rarityColours.Count-3, rarityColours.Count);
            goldToDrop = 10;
            xpToDrop = 30;
            if(randNum<=75){
                crystalsToDrop = 5;
            }
        }else{
            randColour = Random.Range(0, rarityColours.Count-3);
            goldToDrop = 5;
            xpToDrop = 15;
            crystalsToDrop = 0;
        }

        rarityText.GetComponent<TMP_Text>().text = rarityNames[randColour];

        Material material = item.GetComponentInChildren<SpriteRenderer>().material;
        //Debug.Log(material);

        // Set the outline color property of the material to the new color
        material.SetColor("_color", rarityColours[randColour].Evaluate(0)*(randColour+1f));

        var mainModule = item.GetComponentInChildren<ParticleSystem>().colorOverLifetime;
        mainModule.color = new ParticleSystem.MinMaxGradient(rarityColours[randColour]);

        item.GetComponentInChildren<LineRenderer>().startColor = rarityColours[randColour].Evaluate(0);
        item.GetComponentInChildren<LineRenderer>().endColor = rarityColours[randColour].Evaluate(1);
        item.GetComponentInChildren<LineRenderer>().SetPosition(1, new Vector3(0, 0, 0));
        if(randColour > 1){
            StartCoroutine(ShowBeam(item.GetComponentInChildren<LineRenderer>(), new Vector3(0, 0, 0), new Vector3(0, (float)randColour/rarityColours.Count+0.3f, 0), 0.5f));
        }else{
            StartCoroutine(ShowBeam(item.GetComponentInChildren<LineRenderer>(), new Vector3(0, 0, 0), new Vector3(0, 0.6f, 0), 0.5f));
        }
        
        for(int i = 0; i<crystalsToDrop; i++){
            //Debug.Log(i);
            Invoke("InstantiateCrystal", i*0.5f/goldToDrop);
        }

        for(int i = 0; i<goldToDrop; i++){
            //Debug.Log(i);
            Invoke("InstantiateCoin", i*0.5f/goldToDrop);
        }

        //yield return new WaitForSeconds(1);

        foreach(GameObject character in characters){
            xpDestination = character.GetComponent<Attack>().originalPosition;
            StartCoroutine(InstantiateXPSoul(xpDestination, character));
        }

        // xpDestination = currChar.GetComponent<Attack>().originalPosition;
        // for(int i = 0; i<(xpToDrop/3); i++){
        //         //Debug.Log(i);
        //         Invoke("InstantiateXPSoul", i*1f/(xpToDrop/3));
        // }

        yield return new WaitForSeconds(2.8f);
        lootFinished = true;
        Destroy(item, 1);
    }

    IEnumerator ShowBeam(LineRenderer lr, Vector3 startPos, Vector3 endPos, float duration){
        yield return new WaitForSeconds(0.5f);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
                float t = elapsedTime / duration;
                lr.SetPosition(1, Vector3.Lerp(startPos, endPos, t));
                elapsedTime += Time.deltaTime;

                //Debug.Log(startTransform.position);
                yield return null;
        }
        lr.SetPosition(1, endPos);
    }

    IEnumerator NewStage(){
        yield return new WaitForSeconds(1f);
        transitionAnim.SetTrigger("newStage");
        stage += 1;
    }

    void InstantiateCrystal(){
        //Debug.Log("Instantiating coin");
        GameObject coin = Instantiate(crystalPrefab, prevEnemyPos, Quaternion.identity);
        coin.GetComponent<Rigidbody2D>().AddForce((currChar.transform.position-prevEnemyPos + new Vector3(0, 1, 0))*Random.Range(1.5f, 1.8f), ForceMode2D.Impulse);
        StartCoroutine(LerpObject(1.5f, coin.transform, crystalDestination, 1f, false, coin, null, true));
    }

    void InstantiateCoin(){
        //Debug.Log("Instantiating coin");
        GameObject coin = Instantiate(goldCoinPrefab, prevEnemyPos, Quaternion.identity);
        coin.GetComponent<Rigidbody2D>().AddForce((currChar.transform.position-prevEnemyPos + new Vector3(0, 1, 0))*Random.Range(1.5f, 1.8f), ForceMode2D.Impulse);
        StartCoroutine(LerpObject(1.5f, coin.transform, goldDestination, 1f, true, coin, null, false));
    }

    IEnumerator InstantiateXPSoul(Vector3 xpDestination, GameObject player){
        //Debug.Log("Instantiating coin");
        for(int i = 0; i<(xpToDrop/characters.Count); i++){
            yield return new WaitForSeconds(1f/(xpToDrop/characters.Count));
            GameObject soul = Instantiate(xpPrefab, prevEnemyPos, Quaternion.identity);
            StartCoroutine(LerpObject(0, soul.transform, xpDestination, 0.8f, false, soul, player, false));
        }
    }

    IEnumerator LerpObject(float delay, Transform startTransform, Vector3 endingPosition, float lerpDuration, bool isCoin, GameObject obj, GameObject player, bool isCrystal)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;

        Vector3 startingPosition = startTransform.position;
        Vector3 noiseOffset = Random.insideUnitSphere * 0.3f; // Constant noise offset

        while (elapsedTime < lerpDuration)
        {
            float t = elapsedTime / lerpDuration;
            if(isCoin || isCrystal){
                startTransform.position = Vector3.Lerp(startingPosition, endingPosition, t);
            }else{
                // Smoothstep function for smooth acceleration and deceleration
                t = Mathf.SmoothStep(0f, 1f, t);

                // Use Lerp without noise to calculate the position along the straight path
                Vector3 straightPosition = Vector3.Lerp(startingPosition, endingPosition, t);

                // Use Lerp with noise to create a random flight path
                Vector3 newPosition = Vector3.Lerp(straightPosition, endingPosition + noiseOffset, t);

                startTransform.position = straightPosition + noiseOffset;
            }

            elapsedTime += Time.deltaTime;

            //Debug.Log(startTransform.position);
            yield return null;
        }

        if(isCoin){
            gold += 1;
            goldIcon.GetComponent<Animator>().SetTrigger("moreGold");
        }else if(isCrystal){
            crystals += 1;
            crystalIcon.GetComponent<Animator>().SetTrigger("moreGold");
        }else{
            player.GetComponent<Attack>().xp += 1;
        }
        
        Destroy(obj);
    }

    void InstantiateEnemyHealth(GameObject enemy){
        GameObject enemyHealthText = Instantiate(healthText, enemy.transform.position + new Vector3(1.5f, 0f, 0), Quaternion.identity);
        enemyHealthText.transform.SetParent(canvas.transform, false);
        enemyHealthTexts.Add(enemyHealthText);
    }
}
