using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textFloat : MonoBehaviour
{
    private Animator anim;
    private TMP_Text textMeshPro;
    public string damage;

    [SerializeField] private float seconds;
    // Start is called before the first frame update
    void Start()
    { 
        anim = GetComponent<Animator>();
        //Debug.Log(damage);
        textMeshPro = GetComponent<TMP_Text>();
        textMeshPro.text = damage;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(End());
    }

    IEnumerator End(){
        yield return new WaitForSeconds(seconds);
        anim.SetTrigger("end");
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
