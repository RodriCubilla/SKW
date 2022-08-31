using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEnemigo2D : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private float rango;
    [SerializeField] private float danioEso;
    /*private void OnTriggerEnter2D(Collider2D coll) 
    {
        if(coll.CompareTag("Player"))
        {
            print("Daño");
        }    
    }*/

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(controladorDisparo.position, controladorDisparo.right * rango);
    }

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
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
