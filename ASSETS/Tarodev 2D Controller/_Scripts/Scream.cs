using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scream : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("CuadradoGrito"))
        {
            
            Debug.Log("Estoy tocando");
            anim.SetTrigger("Scream");
        }
    }
}
