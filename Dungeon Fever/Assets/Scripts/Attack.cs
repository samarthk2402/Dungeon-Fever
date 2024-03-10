using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject weaponPrefab;
    private GameObject weapon;
    public Transform arms;
    [SerializeField]
    private Weapon w;
    private Animator weaponAnim;
    [SerializeField] 
    private LayerMask enemyLayer;
    [SerializeField] 
    private float moveSpeed;
    [SerializeField] 
    private float moveDistance;
    [SerializeField]
    private GameObject damageText;
    [SerializeField]
    private GameObject damageEffect;

    public int health;
    public int energy;
    public int level;
    public Animator levelup;
    public int xp;

    public Character character;

    public Canvas canvas;
    public Animator transAnim;

    public bool turn;
    public bool turnCompleted = false;

    private Vector3 originalPosition;
    public GameObject player;

    private Animator anim;
    //private bool moveCompleted = false;

    private Vector3 flipScale;
    public bool dead = false;

    public float shakeDuration;
    public float shakeIntensity;

    public float critChance = 10;
    public GameObject critText;

    public enum Option{
        Attack,
        Ability
    }

    public Option option;


    // Start is called before the first frame update
    void Start()
    {
        health = character.health;
        energy = character.energy;
        originalPosition = transform.position;

        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = character.animatorController;

        // Get the current scale of the GameObject
        flipScale = transform.localScale;

            // Flip the scale along the x-axis
        flipScale.x *= -1;

        weapon = Instantiate(weaponPrefab, new Vector3(0, 0.15f, 0), Quaternion.identity);
        weapon.transform.SetParent(arms.transform, false);
        weaponAnim = weapon.GetComponent<Animator>();
        weaponAnim.runtimeAnimatorController = w.animatorController;
    }

    // Update is called once per frame
    void Update()
    {
        if(xp>=30){
            levelup.SetTrigger("levelup");
            level += 1;
            xp = 0;
        }

        if(turn && health>0){
                //Debug.Log(turn);
                if(Input.GetMouseButtonDown(0)){
                    // Cast a ray from the mouse position into the scene
                    Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero, Mathf.Infinity, enemyLayer);

                    // Visualize the raycast
                    //Debug.DrawRay(clickPosition, Vector2.up * 0.1f, Color.red, 1f);

                    // Check if the ray hits any collider on the target layer
                    if (hit.collider != null)
                    {
                        turn = false;
                        // If the ray hits a collider on the target layer, do something
                        GameObject clickedObject = hit.collider.gameObject;
                        //Debug.Log("Clicked object: " + clickedObject.name);

                        StartCoroutine(Move(hit.collider.gameObject));
                    }

                }
        }

        if(health<=0){
            transform.localScale = flipScale;

            Vector3 targetPos;

            targetPos = originalPosition + new Vector3(-15, 0);

            StartCoroutine(MoveOffScreen(originalPosition, targetPos, moveSpeed/20));

        }
    }

    private IEnumerator MoveOffScreen(Vector3 startPos, Vector3 endPos, float duration)
    {
        //yield return new WaitForSeconds(1);

        dead = true;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            //Debug.Log(transform.position);
            yield return null;
        }

        // Ensure the object reaches exactly the end position
        transform.position = endPos;

        // Destroy the object if it's close enough to the target position
        if (Vector3.Distance(transform.position, endPos) < 1)
        {
            Destroy(gameObject);
        }
    }

    void DamageEnemy(GameObject enemy){
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        Bounds sb = sr.bounds;
                        
        GameObject inst;
        inst = Instantiate(damageText, enemy.transform.position, Quaternion.identity);
        inst.transform.SetParent(canvas.transform, false);
        inst.GetComponent<textFloat>().colour = Color.white;

        GameObject ps = Instantiate(damageEffect, enemy.transform.position + new Vector3(0, -(sb.size.y/2), 0), Quaternion.Euler(-90, 0, 0));
        Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);

        StartCoroutine(enemy.GetComponent<EnemyAttack>().ShakeCoroutine());

        switch(option){
            case Option.Attack:
                inst.GetComponent<textFloat>().damage = ((character.strength + w.damage) * Crit(enemy)).ToString();
                enemy.GetComponent<EnemyAttack>().health -= (character.strength + w.damage) * Crit(enemy);
                break;
            case Option.Ability:
                GameObject ae = Instantiate(character.abilityEffect, transform.position, Quaternion.identity);
                Destroy(ae, ae.GetComponent<ParticleSystem>().main.duration);
                energy -= character.abilityCost;
                int d = character.abilityDamage * Crit(enemy);
                inst.GetComponent<textFloat>().damage = d.ToString();
                enemy.GetComponent<EnemyAttack>().health -= d;
                break;
        }
    }

    int Crit(GameObject enemy){
        int randomNumber = Random.Range(0, 100);

        // Check if the random number falls within the execution chance
        if (randomNumber < critChance)
        {
            GameObject crit;
            crit = Instantiate(critText, enemy.transform.position+new Vector3(0, 2, 0), Quaternion.identity);
            crit.GetComponentInChildren<textFloat>().damage = "Crit";
            crit.GetComponentInChildren<textFloat>().colour = Color.red;
            crit.transform.SetParent(canvas.transform, false);
            Destroy(crit, 2);

            transAnim.SetTrigger("critical");
            return 2;
        }else{
            return 1;
        }
    }

    IEnumerator Move(GameObject enemy)
    {
        Vector3 startingPosition = transform.position;
        Vector3 direction = enemy.transform.position - transform.position;
        direction.Normalize();
        float distance = Mathf.Abs(Vector3.Distance(transform.position, enemy.transform.position-(direction*moveDistance)));
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, startingPosition + direction*distance, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly the stopping distance away
        transform.position = enemy.transform.position-(direction*moveDistance);

        // Wait for a short duration before returning to the original position
        switch(option){
            case Option.Attack:
                anim.SetTrigger("attack");
                weaponAnim.SetTrigger("attack");
                break;
            case Option.Ability:
                anim.SetTrigger("ability");
                weapon.SetActive(false);
                StartCoroutine(ReactivateWeapon());
                break;
        }

        Vector3 currPos = transform.position;

        yield return new WaitForSeconds(0.3f);
        DamageEnemy(enemy);
        yield return new WaitForSeconds(0.5f);
        //weapon.SetActive(true);

        // Move back to the original position
        float returnDuration = Vector3.Distance(transform.position, originalPosition) / moveSpeed;
        elapsedTime = 0f;
        while (elapsedTime < returnDuration)
        {
            transform.position = Vector3.Lerp(currPos, originalPosition, elapsedTime / returnDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly the original position
        transform.position = originalPosition;
        turnCompleted = true;
    }
    
    IEnumerator ReactivateWeapon(){
        yield return new WaitForSeconds(0.8f);
        weapon.SetActive(true);
    }

    public void SetAttack(){
        option = Option.Attack;
    }

    public void SetAbility(){
        option = Option.Ability;
    }

    public IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            transform.position = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Reset the object's position after shake duration ends
        transform.position = originalPosition;
    }
}
