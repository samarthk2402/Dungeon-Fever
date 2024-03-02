using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsManager : MonoBehaviour
{
    public GameObject player;
    public Button AbilityButton;

    void Update(){
        if(player != null){
            if(player.GetComponent<Attack>().energy <= 0){
                AbilityButton.interactable = false;
            }else{
                AbilityButton.interactable = true;
            }
        }
    }

    public void Restart(){
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the scene by passing its index
        SceneManager.LoadScene(currentSceneIndex);
    }
}
