using System.Collections;
using UnityEngine;

public class movimiento : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 direction;

    [Header("Componentes")]
    [SerializeField] private Rigidbody2D _rb;

    void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate() {
        movePersonaje(direction.x);    
    }
    void movePersonaje(float horizontal)
    {
        _rb.AddForce(Vector2.right * horizontal * moveSpeed);
    }
}