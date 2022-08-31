using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [SerializeField] private float danioPinchos = 15f;
    [SerializeField] private float tiempoEntreDanio = 1f;
    private float tiempoSiguienteDanio;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            tiempoSiguienteDanio -= Time.deltaTime;
            if(tiempoSiguienteDanio <= 0)
            {
                other.GetComponent<PlayerStats>().DanioESO(danioPinchos);
                tiempoSiguienteDanio = tiempoEntreDanio;
            }
        }
    }
}
