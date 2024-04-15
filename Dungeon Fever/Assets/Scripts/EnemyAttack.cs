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
    private GameObject slashEffect;
    [SerializeField]
    public Canvas canvas;

    [SerializeField]
    public Enemy enemy;

    [SerializeField]
    private GameObject shadow;

    public List<GameObject> characters = new List<GameObject>();

    public bool turn;
    public bool turnCompleted = false;

    private Vector3 originalPosition;

    public Animator anim;

    public int health;

    private Vector3 bottomPos;

    public float shakeIntensity;
    public float shakeDuration;

    private bool moving;
    public bool rare;
    public bool superRare;

    public Material RareMat;
    public Material SuperRareMat;
    public Material defaultMat;
    public Material dissolveMat;
    public float dissolveDuration = 2f;
    public Color rareCol;
    public Color superRareCol;
    public Gradient rareGrad;
    public Gradient superRareGrad;
    public GameObject ps;
    private static readonly int colour = Shader.PropertyToID("_color");
    public bool dead = false;

    public string special;
    // Start is called before the first frame update
    void Start()
    {
        special = enemy.special;
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = enemy.animatorController;
        Renderer rend = GetComponent<Renderer>();
        defaultMat = rend.material;
        if(rare){
            ps.SetActive(true);
            rend.material = RareMat;
            health = Mathf.RoundToInt(enemy.health*1.5f);

            var mainModule = ps.GetComponent<ParticleSystem>().colorOverLifetime;
            mainModule.color = new ParticleSystem.MinMaxGradient(rareGrad);
        }else if(superRare){
            ps.SetActive(true);
            rend.material = SuperRareMat;
            health = Mathf.RoundToInt(enemy.health*2f);

            var mainModule = ps.GetComponent<ParticleSystem>().colorOverLifetime;
            mainModule.color = new ParticleSystem.MinMaxGradient(superRareGrad);
        }else{
            ps.SetActive(false);
            rend.material = defaultMat;
            health = Mathf.RoundToInt(enemy.health);
        } 
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(sr.color);
        bottomPos = new Vector3(0, -GetComponent<SpriteRenderer>().bounds.size.y/2, 0);
        shadow.transform.position = transform.position + bottomPos;

        if(turn && health > 0){
            //turn = false;
            if(!moving){
                int randPlayer = Random.Range(0, characters.Count);
                if(characters.Count>0){
                    if(characters[randPlayer] != null && !characters[randPlayer].GetComponent<Attack>().dead){
                        StartCoroutine(Move(characters[randPlayer]));
                        moving = true;
                    }
                }
                
            }
        }

        if(health <= 0){
            //anim.SetTrigger("death");
            //Destroy(this.gameObject);
            if(!dead){
                dead = true;
                StartCoroutine(Death());
            }
        }
    }

    IEnumerator Death(){
        shadow.SetActive(false);
        float elapsedTime = 0f;
        Renderer enemyRenderer = GetComponent<Renderer>();
        enemyRenderer.material = dissolveMat;

        while (elapsedTime < dissolveDuration)
        {
            float fadeValue = Mathf.Lerp(1f, 0f, elapsedTime / dissolveDuration);
            enemyRenderer.material.SetFloat("_Fade", fadeValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure fade value is fully 0 and revert material
        enemyRenderer.material.SetFloat("_Fade", 0f);
    }

    IEnumerator Move(GameObject player)
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
        Attack(player);
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

    void Attack(GameObject player){
            //Debug.Log("Enemy Attack");
            turn = false;

            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            Bounds sb = sr.bounds;

            GameObject ps = Instantiate(damageEffect, player.transform.position + new Vector3(0, -(sb.size.y/2), 0), Quaternion.Euler(-90, 0, 0));
            Destroy(ps, ps.GetComponent<ParticleSystem>().main.duration);

            GameObject inst = Instantiate(damageText, player.transform.position+new Vector3(0, 1, 0), Quaternion.identity);
            inst.transform.SetParent(canvas.transform, false);

            GetComponent<AudioSource>().Play();
            
            if(rare){
                inst.GetComponentInChildren<textFloat>().damage = Mathf.RoundToInt(enemy.damage*1.5f).ToString();
                inst.GetComponentInChildren<textFloat>().colour = rareCol;
                player.GetComponent<Attack>().health -= Mathf.RoundToInt(enemy.damage*1.5f);
            }else if(superRare){
                inst.GetComponentInChildren<textFloat>().damage = Mathf.RoundToInt(enemy.damage*2f).ToString();
                inst.GetComponentInChildren<textFloat>().colour = superRareCol;
                player.GetComponent<Attack>().health -= Mathf.RoundToInt(enemy.damage*2f);
            }else{
                inst.GetComponentInChildren<textFloat>().damage = Mathf.RoundToInt(enemy.damage).ToString();
                inst.GetComponentInChildren<textFloat>().colour = Color.white;
                player.GetComponent<Attack>().health -= enemy.damage;
            }

            GameObject slash;
            slash = Instantiate(slashEffect, player.transform.position, Quaternion.identity);
            Vector3 currentScale = slash.transform.localScale;

            // Invert the X scale to flip horizontally
            currentScale.x *= -1;

            // Apply the new local scale
            slash.transform.localScale = currentScale;
            Destroy(slash, 0.3f);

            StartCoroutine(player.GetComponent<Attack>().ShakeCoroutine());   
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
