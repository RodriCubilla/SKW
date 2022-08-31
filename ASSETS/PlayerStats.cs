using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //CUALIDADES
    [Header("Personaje")] 
    public float vida = 150; //salud
    private Rigidbody2D rb;
    private SpriteRenderer spritePersonaje;

    public GameObject GameOverUI;
    public CombateCaC Combate;
    //public PlayerInput movementPersonaje;

    [SerializeField] private Enemigos script;

    Animator animacion;

    //SOLO UNA VEZ AL EMPEZAR
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animacion = GetComponent<Animator>();
        spritePersonaje = GetComponent<SpriteRenderer>();
        script.enabled = true;
        GameOverUI.SetActive(false);
        Time.timeScale = 1;
    }

    //CADA FRAME
    void Update()
    {  
    }

    //<---------DAÑO Y MUERTE--------->//
    public void DanioESO(float danioESO)
    {
        animacion.SetTrigger("Damage");
        vida -= danioESO;

        if(vida <= 0)
        {
            Muerte();
            script.enabled = false;
        }
    }

    

    private void Muerte()
    {
        animacion.SetBool("Death", true);
        //GameOverUI.SetActive(true);
        Combate.enabled = false;
        StartCoroutine(FinJuego());
        
    }

    IEnumerator FinJuego()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        GameOverUI.SetActive(true);
    }
}
