using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombateCaC : MonoBehaviour
{
    [Header("Combate CaC")]
    [SerializeField] private Transform controladorGolpe;
    [SerializeField] private Transform controladorCola;
    [SerializeField] private Transform controladorGolpeAire;
    [SerializeField] private float radioGolpeGarras;
    [SerializeField] private float radioGolpeCola;
    [SerializeField] private float radioGolpeAire;
    [SerializeField] private float dañoGolpeGarras;
    [SerializeField] private float dañoGolpeCola;
    [SerializeField] private float dañoGolpeAire;
    private Animator animator;
    [SerializeField] private float tiempoEntreAtaques;
    [SerializeField] private float tiempoSiguienteAtaque;

    [SerializeField] private LayerMask queEsSuelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCajaSuelo;
    private bool enSuelo;
    public bool ataca = false;


    //<---------START--------->//
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    
    //<---------UPDATE--------->//
    private void Update()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCajaSuelo, 0f, queEsSuelo);

        if(tiempoSiguienteAtaque > 0)
        {
            tiempoSiguienteAtaque -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Fire1") && tiempoSiguienteAtaque <= 0 && enSuelo)
        {
            Golpe();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
        else if(Input.GetButtonDown("Fire2") && tiempoSiguienteAtaque <= 0 && enSuelo)
        {
            GolpeCola();
            tiempoSiguienteAtaque = tiempoEntreAtaques;
        }
        else if(Input.GetButtonDown("Fire1") && enSuelo == false)
        {
            GolpeAire();
        }
    }
    
    //<---------ATAQUE--------->//
    private void Golpe()
    {
        animator.SetTrigger("Ataque");
        ataca = true;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpeGarras);

        foreach (Collider2D collisionador in objetos)
        {
            if(collisionador.CompareTag("Enemy"))
            {
                collisionador.transform.GetComponent<EnemyHealth>().TomarDaño(dañoGolpeGarras);
            }
        }
        StartCoroutine(AtaqueFalse());
    }

    private void GolpeCola()
    {
        animator.SetTrigger("AtaqueCola");
        ataca = true;

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpeCola);

        foreach (Collider2D collisionador in objetos)
        {
            if(collisionador.CompareTag("Enemy"))
            {
                collisionador.transform.GetComponent<EnemyHealth>().TomarDaño(dañoGolpeCola);
            }
        }
        StartCoroutine(AtaqueFalse());
    }

    public void GolpeAire()
    {
        animator.SetTrigger("AtaqueAire");

        Collider2D[] objetos = Physics2D.OverlapCircleAll(controladorGolpe.position, radioGolpeAire);

        foreach (Collider2D collisionador in objetos)
        {
            if(collisionador.CompareTag("Enemy"))
            {
                collisionador.transform.GetComponent<EnemyHealth>().TomarDaño(dañoGolpeAire);
            }
        }
    }

    IEnumerator AtaqueFalse()
    {
        yield return new WaitForSeconds(.5f);
        ataca = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(controladorGolpe.position, radioGolpeGarras);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(controladorCola.position, radioGolpeCola);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(controladorGolpeAire.position, radioGolpeAire);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCajaSuelo);
    }
}
