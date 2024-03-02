using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newStage : MonoBehaviour
{
    public ManageGameStat gsScript;

    // Start is called before the first frame update
    void Start()
    {
        gsScript = GetComponentInParent<ManageGameStat>();
    }

    public void NewStage(){
        gsScript.InstantiateEnemy();
        Debug.Log("Instantiate Enemy");
    }
}
