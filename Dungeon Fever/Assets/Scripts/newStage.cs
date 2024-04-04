using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            gsScript.InstantiateEnemy(gsScript.enemyPositions[1], false);
        }else if(stage<=6){
            gsScript.InstantiateEnemy(gsScript.enemyPositions[0], false);
            gsScript.InstantiateEnemy(gsScript.enemyPositions[2], false);
        }else if(stage<=9){
            gsScript.InstantiateEnemy(gsScript.enemyPositions[0], false);
            gsScript.InstantiateEnemy(gsScript.enemyPositions[1], false);
            gsScript.InstantiateEnemy(gsScript.enemyPositions[2], false);
        }else if(stage == 10){
            gsScript.InstantiateEnemy(gsScript.enemyPositions[1], true);
        }else if(stage >= 11){
            SceneManager.LoadScene(2);
        }
    }
}
