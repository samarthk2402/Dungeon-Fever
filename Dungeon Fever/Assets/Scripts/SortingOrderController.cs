using UnityEngine;

public class SortingOrderController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on GameObject: " + gameObject.name);
            return;
        }
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }

    void LateUpdate()
    {
        // Set sorting order based on Y position
        spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }
}
