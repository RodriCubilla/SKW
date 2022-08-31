using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambioDireccion : MonoBehaviour
{
    [SerializeField] private Transform controladorPared;
    [SerializeField] private float distancia;
    [SerializeField] private bool moviendoDerecha;
    [SerializeField]  LayerMask pared;
    private Rigidbody2D _rb;
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        RaycastHit2D hayPared = Physics2D.Raycast(controladorPared.position, controladorPared.right, distancia);

        if(hayPared == true)
        {
            Girar();
        }
    }

    private void Girar()
    {
        moviendoDerecha = !moviendoDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(controladorPared.transform.position, controladorPared.transform.position + Vector3.right * distancia);
        
    }
}
