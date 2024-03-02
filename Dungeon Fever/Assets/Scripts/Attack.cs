using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
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
    public int damage;
    public int abilityDamage;
    public int abilityCost;
    public int health;
    public int energy;
    [SerializeField]
    private Canvas canvas;

    public bool turn;
    public bool turnCompleted = false;

    private Vector3 originalPosition;
    public GameObject player;

    private Animator anim;
    //private bool moveCompleted = false;

    private Vector3 flipScale;
    public bool dead = false;

    public enum Option{
        Attack,
        Ability
    }

    public Option option;


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;

        anim = GetComponent<Animator>();

        // Get the current scale of the GameObject
        flipScale = transform.localScale;

            // Flip the scale along the x-axis
        flipScale.x *= -1;
    }

    // Update is called once per frame
    void Update()
    {

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

                        SpriteRenderer sr = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                        Bounds sb = sr.bounds;
                        
                        GameObject inst;
                        inst = Instantiate(damageText, hit.collider.gameObject.transform.position, Quaternion.identity);
                        inst.transform.SetParent(canvas.transform, false);

                        GameObject ps = Instantiate(damageEffect, hit.collider.gameObject.transform.position + new Vector3(0, -(sb.size.y/2), 0), Quaternion.Euler(-90, 0, 0));
                        Debug.Log(ps);
                        Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);

                        StartCoroutine(hit.collider.gameObject.GetComponent<EnemyAttack>().ShakeCoroutine());

                        switch(option){
                            case Option.Attack:
                                StartCoroutine(Move("attack"));
                                inst.GetComponent<textFloat>().damage = damage;
                                hit.collider.gameObject.GetComponent<EnemyAttack>().health -= damage;
                                break;
                            case Option.Ability:
                                energy -= abilityCost;
                                StartCoroutine(Move("ability"));
                                inst.GetComponent<textFloat>().damage = abilityDamage;
                                hit.collider.gameObject.GetComponent<EnemyAttack>().health -= abilityDamage;
                                break;
                        }
                    }

                }
        }

        if(health<=0){
            transform.localScale = flipScale;

            Vector3 targetPos;

            targetPos = originalPosition + new Vector3(-15, 0);

            StartCoroutine(MoveOffScreen(originalPosition, targetPos, moveSpeed/2));

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
    IEnumerator Move(string animation)
    {
        float distance = Mathf.Abs(moveDistance);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, startingPosition + new Vector3(moveDistance, 0, 0), elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly the stopping distance away
        transform.position = startingPosition + new Vector3(moveDistance, 0, 0);

        // Wait for a short duration before returning to the original position
        anim.SetTrigger(animation);
        yield return new WaitForSeconds(0.5f);

        // Move back to the original position
        float returnDuration = Vector3.Distance(transform.position, originalPosition) / moveSpeed;
        elapsedTime = 0f;
        while (elapsedTime < returnDuration)
        {
            transform.position = Vector3.Lerp(startingPosition + new Vector3(moveDistance, 0, 0), originalPosition, elapsedTime / returnDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly the original position
        transform.position = originalPosition;
        turnCompleted = true;
    }

    public void SetAttack(){
        option = Option.Attack;
    }

    public void SetAbility(){
        option = Option.Ability;
    }
}
