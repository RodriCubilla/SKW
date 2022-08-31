using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigos : MonoBehaviour
{
    //<---------DECLARACIONES--------->//
    [Header("PATRULLA")]
    [SerializeField] private Transform controladorPared;
    [SerializeField] private string enemyName;
    [SerializeField] private float velocidad;
    [SerializeField] private float distancia; 
    RaycastHit hit;
    [SerializeField] private LayerMask LayerM;
    [SerializeField] private bool Right;
    private SpriteRenderer spriteEnemy;

    private Animator animator;


    [Header("COMBATE")]
    [SerializeField] private Transform player_pos;
    private PlayerStats scriptESO;
    [SerializeField] private float distancia_eso;
    [SerializeField] private float distance_eso_back;
    [SerializeField] private float altura_eso;
    
    [SerializeField] private Transform controladorDistanciaEso;
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private float rango;
    //[SerializeField] private GameObject efectoImpacto;
    //[SerializeField] private LineRenderer disparo;
    [SerializeField] private float tiempoDisparo;
    [SerializeField] private float danioEso;
    private float timer;
    

    //<---------START--------->//
    private void Start()
    {
        player_pos = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        scriptESO = GameObject.Find("Player").GetComponent<PlayerStats>();
        spriteEnemy = GetComponent<SpriteRenderer>();

        InvokeRepeating("Comportamiento", 0, 3f);
        InvokeRepeating("Velocidad", 0, 3f);
    }

    
    //<---------PLATAFORMAS--------->//
    void OnDrawGizmos() 
    {
        //Mide distancia a pared//
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(controladorPared.position, transform.right * distancia);
        //Mide la distancia del ESO en la espalda//
        Gizmos.color = Color.green;
        Gizmos.DrawRay(controladorDistanciaEso.position, transform.right * distance_eso_back);
        //Mide la distancia del ESO al frente//
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(controladorDisparo.position, controladorDisparo.right * distancia_eso);
        //Mide el rango del disparo del eso//
        Gizmos.color = Color.red;
        Gizmos.DrawRay(controladorDisparo.position, controladorDisparo.right * rango);
        //Distancia para arriba//
        Gizmos.color = Color.white;
        Gizmos.DrawRay(controladorDisparo.position, controladorDisparo.up * altura_eso);
    }
    
    //<---------UPDATE--------->//
    void Update()
    {
        animator.SetFloat("Velocidad", velocidad);

        Distancia();

        Apunta();

        RaycastHit2D rayleft = Physics2D.Raycast(controladorDistanciaEso.position, controladorDistanciaEso.right, distance_eso_back);

        if(rayleft)
        {
            if(rayleft.transform.CompareTag("Player"))
            {
                this.transform.rotation = Quaternion.Euler (0, 180, 0);
            }
        }
    }

    void Velocidad() => velocidad = velocidad == 80 ? 0 : 80;
    
    //<---------DISPARAR--------->//
    private void Disparar()
    {
        //<------------EL RAYCAST DETECTA AL PERSONAJE Y LO HIERE---------------->
        RaycastHit2D raycastHit2D = Physics2D.Raycast(controladorDisparo.position, controladorDisparo.right, rango);

        if(raycastHit2D)
        {
            if(raycastHit2D.transform.CompareTag("Player"))
            {
                raycastHit2D.transform.GetComponent<PlayerStats>().DanioESO(danioEso);
                //Instantiate(efectoImpacto, raycastHit2D.point, Quaternion.identity);
                //StartCoroutine(GenerarLinea(raycastHit2D.point));
            }
        }


        //<---------GIRA SEGÚN POSICIÓN DEL PERSONAJE----------->
        if(this.transform.position.x < player_pos.position.x)
        {  
            this.transform.rotation = Quaternion.Euler (0, 0, 0);
        }
        else if(this.transform.position.x > player_pos.position.x)
        {
            this.transform.rotation = Quaternion.Euler(0, 180,0);
        }
    }
    // IEnumerator GenerarLinea(Vector3 objetivo)
    // {
    //     disparo.enabled = true;
    //     disparo.SetPosition(0, controladorDisparo.position);
    //     disparo.SetPosition(1, objetivo);
    //     yield return new WaitForSeconds(tiempoDisparo);
    //     disparo.enabled = false;
    // }

    private void Distancia()
    {
        float disX;
        float disY;

        disX = controladorDisparo.transform.position.x - player_pos.position.x;
        disY = controladorDisparo.transform.position.y - player_pos.position.y;

        if(disX < distancia_eso && disY < altura_eso)
        {
            velocidad = 0;
            animator.SetBool("Dispara", true);
        }
        else
        {   
            if (Right)
            {
                transform.rotation = Quaternion.Euler (0, 180, 0);
                transform.Translate(Vector3.right * velocidad * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.Translate(Vector3.right * velocidad * Time.deltaTime);
            }

            if (Physics2D.Raycast(transform.position, transform.right, distancia, LayerM))
            {
                Right =! Right;
            }
        }

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else 
        {
            timer = 1;
            Disparar();
        }
    }

    public void Apunta()
    {
        if(scriptESO.vida <= 0)
        {
            animator.SetBool("Dispara", false);
            animator.SetTrigger("Apunta");
        }
    }

    public void Comportamiento()
    {
        animator.SetInteger("Idle", Random.Range(-1, 4));        
    }
}
