using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{   
    [SerializeField] private float vida;
    [SerializeField] private Enemigos Disparar;

    private Animator animator;
    private Rigidbody2D rb; 

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Disparar.enabled = true;
    }

    public void TomarDaño(float dañoGarras)
    {
        animator.SetTrigger("GOLPE");
        Disparar.enabled = false;
        
        vida -= dañoGarras;
        if(vida <= 0)
        {
            Muerte();
        }
        else if(vida >= 0)
        {
            SoldadoLevanta();
            StartCoroutine(EmpiezaAtaque());
        }
    }

    private void SoldadoLevanta()
    {
        animator.SetTrigger("GolpeCola");
        rb.AddForce(transform.up * 10f,  ForceMode2D.Impulse);
        AfterUp();
    }
    
    public void AfterUp()
    {
        animator.SetInteger("AfterUp", Random.Range(0, 3));  
    }

    private void Muerte()
    {  
        animator.SetTrigger("DEATH");
        Destroy(gameObject, 4);
    }

    IEnumerator EmpiezaAtaque()
    {
        yield return new WaitForSeconds(4f);
        GetComponent<Enemigos>().Apunta();
        Disparar.enabled = true;
    }
}
