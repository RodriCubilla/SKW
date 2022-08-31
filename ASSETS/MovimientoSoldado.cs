using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoSoldado : MonoBehaviour
{
    //<---------DECLARACIONES--------->//
    [Header("PATRULLA")]
    [SerializeField] private string enemyName;
    [SerializeField] private float velocidad;
    [SerializeField] private float distancia; 
    RaycastHit hit;
    [SerializeField] private LayerMask LayerM;
    [SerializeField] private bool Left;
    private SpriteRenderer spriteEnemy;
    private int rutina;
    private float cronometro;
    private int direccion;

    private Animator anim;

    [Header("COMBATE")]
    [SerializeField] private Transform player_pos;
    [SerializeField] private Transform controladorDistanciaEso;
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private float distancia_eso;
    [SerializeField] private float rango;
    [SerializeField] private float distance_eso_back;
    [SerializeField] private float altura_eso;
    //[SerializeField] private GameObject efectoImpacto;
    //[SerializeField] private LineRenderer disparo;
    [SerializeField] private float tiempoDisparo;
    [SerializeField] private float danioEso;
    private float timer;

    //<---------START--------->//
    private void Start()
    {
        player_pos = GameObject.Find("Player").transform;
        anim = GetComponent<Animator>();
        spriteEnemy = GetComponent<SpriteRenderer>();
    }

    
    //<---------PLATAFORMAS--------->//
    void OnDrawGizmos() 
    {
        //Mide distancia a pared//
        /*Gizmos.color = Color.blue;
        Gizmos.DrawRay(controladorPared.position, controladorPared.right * distancia);*/
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
        Distancia();

        RaycastHit2D rayleft = Physics2D.Raycast(controladorDistanciaEso.position, controladorDistanciaEso.right, distance_eso_back);
        //RaycastHit2D hayPared = Physics2D.Raycast(controladorPared.position, Vector2.right, distancia);

        /*if(hayPared == true)
        {
            Girar();
        }*/

        if(rayleft)
        {
            if(rayleft.transform.CompareTag("Player"))
            {
                this.transform.rotation = Quaternion.Euler(0,180,0);
            }
        }
    }
    
    //<---------DISPARAR--------->//
    private void Disparar()
    {
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
            anim.SetBool("Walk", false);
            anim.SetTrigger("Dispara2");        
        }
        else
        {
           cronometro += 1 * Time.deltaTime;
            if(cronometro >= 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    anim.SetBool("Walk", false);
                    break;

                case 1:
                    direccion = Random.Range(0, 2);
                    rutina++;
                    break;

                case 2:

                    switch (direccion)
                    {
                        case 0:
                            transform.rotation = Quaternion.Euler(0, 180, 0);
                            transform.Translate(Vector3.right * velocidad * Time.deltaTime);
                        break;

                        case 1:
                            transform.rotation = Quaternion.Euler(0, 0, 0);
                            transform.Translate(Vector3.right * velocidad * Time.deltaTime);
                        break;
                    }
                    anim.SetBool("Walk", true);
                    break;
                case 3:
                    if(Physics2D.Raycast(transform.position, transform.right, distancia, LayerM))
                    {
                        Left =! Left;
                    }
                    break;
            }

            /*if (Physics2D.Raycast(controladorPared.position, controladorPared.right, distancia, LayerM))
            {
                Left =! Left;
            }*/
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
        anim.SetBool("Apunta", true);
    }

    // private void Girar()
    // {
    //     transform.rotation = Quaternion.Euler(0, 0, 0);
    //     transform.Translate(Vector3.right * velocidad * Time.deltaTime);
    // }
}