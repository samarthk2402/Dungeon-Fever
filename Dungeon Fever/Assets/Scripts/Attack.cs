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
    public int damage;
    public int health;
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private bool isEnemy;

    public bool turn;
    public bool turnCompleted = false;

    private Vector3 originalPosition;
    public GameObject player;

    private Animator anim;
    //private bool moveCompleted = false;

    private Vector3 flipScale;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        if(isEnemy){
            moveDistance *= -1;
        }

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
            if(!isEnemy){
                //Debug.Log(turn);
                if(Input.GetMouseButtonDown(0)){
                    //turn = false;
                    // Cast a ray from the mouse position into the scene
                    Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(clickPosition, Vector2.zero, Mathf.Infinity, enemyLayer);

                    // Visualize the raycast
                    //Debug.DrawRay(clickPosition, Vector2.up * 0.1f, Color.red, 1f);

                    // Check if the ray hits any collider on the target layer
                    if (hit.collider != null)
                    {
                        // If the ray hits a collider on the target layer, do something
                        GameObject clickedObject = hit.collider.gameObject;
                        //Debug.Log("Clicked object: " + clickedObject.name);
                        
                        // Lerp towards position
                        StartCoroutine(Move());
                        GameObject inst = Instantiate(damageText, hit.collider.gameObject.transform.position, Quaternion.identity);
                        inst.transform.SetParent(canvas.transform, false);
                        inst.GetComponent<textFloat>().damage = damage;
                        hit.collider.gameObject.GetComponent<Attack>().health -= damage;
                        turnCompleted = true;
                    }

                }
            }else{
                turn = false;
                StartCoroutine(EnemyAttack(1.5f));
            }
        }

        if(health<=0){
            transform.localScale = flipScale;

            Vector3 targetPos;

            if(isEnemy){
                targetPos = originalPosition + new Vector3(15, 0);
            }else{
                targetPos = originalPosition + new Vector3(-15, 0);
            }

            StartCoroutine(MoveOffScreen(originalPosition, targetPos, moveSpeed));

        }
    }
    private IEnumerator MoveOffScreen(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
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
    
    IEnumerator EnemyAttack(float delay){
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
