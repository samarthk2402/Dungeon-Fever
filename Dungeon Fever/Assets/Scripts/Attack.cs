using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] 
    private LayerMask enemyLayer;
    [SerializeField] 
    private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if the ray hits any collider on the target layer
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
            {
                // If the ray hits a collider on the target layer, do something
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log("Clicked object: " + clickedObject.name);
                
                // Add your custom logic here
            }
        }
    }
}
