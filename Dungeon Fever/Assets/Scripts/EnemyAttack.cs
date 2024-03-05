using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] 
    private float moveSpeed;
    [SerializeField] 
    private float moveDistance;
    [SerializeField]
    private GameObject damageText;
    [SerializeField]
    private GameObject damageEffect;
    [SerializeField]
    public Canvas canvas;

    [SerializeField]
    public Enemy enemy;

    [SerializeField]
    private GameObject shadow;

    public GameObject player;

    public bool turn;
    public bool turnCompleted = false;

    private Vector3 originalPosition;

    public Animator anim;

    public int health;

    private Vector3 bottomPos;

    public float shakeIntensity;
    public float shakeDuration;

    private bool moving;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = enemy.animatorController;
        health = enemy.health;
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bottomPos = new Vector3(0, -GetComponent<SpriteRenderer>().bounds.size.y/2, 0);
        shadow.transform.position = transform.position + bottomPos;

        if(turn && health > 0){
            //turn = false;
            if(!moving){
                StartCoroutine(Move());
                moving = true;
            }
        }

        if(health <= 0){
            shadow.SetActive(false);
            anim.SetTrigger("death");
            //Destroy(this.gameObject);
        }
    }

    void Death(){
        Destroy(this.gameObject);
    }

    IEnumerator Move()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 startingPosition = transform.position;
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        float distance = Mathf.Abs(Vector3.Distance(transform.position, player.transform.position-(direction*moveDistance)));
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, startingPosition + direction*distance, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure we reach exactly the stopping distance away
        transform.position = player.transform.position-(direction*moveDistance);

        //Debug.Log("forward");
        // Wait for a short duration before returning to the original position
        anim.SetTrigger("attack");
        Vector3 currPos = transform.position;

        yield return new WaitForSeconds(0.3f);
        if(turn){
            Attack();
        }
        yield return new WaitForSeconds(0.5f);

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
        moving = false;
        //Debug.Log("back");
    }

    void Attack(){
            //Debug.Log("Enemy Attack");
            turn = false;

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            Bounds sb = sr.bounds;

            GameObject ps = Instantiate(damageEffect, player.transform.position + new Vector3(0, -(sb.size.y/2), 0), Quaternion.Euler(-90, 0, 0));
            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);

            GameObject inst = Instantiate(damageText, player.transform.position, Quaternion.identity);
            inst.transform.SetParent(canvas.transform, false);
            inst.GetComponent<textFloat>().damage = enemy.damage.ToString();

            StartCoroutine(player.GetComponent<Attack>().ShakeCoroutine());

            player.GetComponent<Attack>().health -= enemy.damage;    
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
