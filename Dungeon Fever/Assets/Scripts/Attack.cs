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

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
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
            }
        }
    }

    IEnumerator Move()
    {
        float distance = moveDistance;
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
        //yield return new WaitForSeconds(0.5f);

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
}
