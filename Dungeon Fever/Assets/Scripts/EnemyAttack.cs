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
    public int damage;
    public int health;
    [SerializeField]
    private Canvas canvas;

    public GameObject player;

    public bool turn;
    public bool turnCompleted = false;

    private Vector3 originalPosition;

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        moveDistance *= -1;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(turn && health > 0){
            turn = false;
            StartCoroutine(Attack(1.5f));
        }

        if(health <= 0){
            Destroy(this.gameObject);
        }
    }

    IEnumerator Move()
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
        anim.SetTrigger("attack");
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
    }

    IEnumerator Attack(float delay){
        yield return new WaitForSeconds(delay);
        if(!turnCompleted){
            turn = false;
            StartCoroutine(Move());
            GameObject inst = Instantiate(damageText, player.transform.position, Quaternion.identity);
            inst.transform.SetParent(canvas.transform, false);
            inst.GetComponent<textFloat>().damage = damage;
            player.GetComponent<Attack>().health -= damage; 
            turnCompleted = true;
        }        
    }
}
