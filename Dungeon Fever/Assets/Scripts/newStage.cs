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
        foreach(Vector3 pos in gsScript.enemyPositions){
            gsScript.InstantiateEnemy(pos);
        }
    }
}
