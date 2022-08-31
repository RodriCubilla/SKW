using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorScript : MonoBehaviour
{
    private Animator anim;

    [SerializeField] private string numEscena;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    IEnumerator OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player")
        {
            anim.SetTrigger("Ascensor");
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(numEscena);
        }
    }
}
