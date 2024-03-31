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
        // foreach(Vector3 pos in gsScript.enemyPositions){
        //     gsScript.InstantiateEnemy(pos);
        // }
        int stage = gsScript.stage;
        if(stage<=3){
            gsScript.InstantiateEnemy(gsScript.enemyPositions[1]);
        }else if(stage<=6){
            gsScript.InstantiateEnemy(gsScript.enemyPositions[0]);
            gsScript.InstantiateEnemy(gsScript.enemyPositions[2]);
        }else if(stage<=9){
            gsScript.InstantiateEnemy(gsScript.enemyPositions[0]);
            gsScript.InstantiateEnemy(gsScript.enemyPositions[1]);
            gsScript.InstantiateEnemy(gsScript.enemyPositions[2]);
        }
    }
}
