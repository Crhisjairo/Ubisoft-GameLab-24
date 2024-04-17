using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Controllers;
using Mirror;
using UnityEngine;

public class Spike : NetworkBehaviour
{
    public float speed = 3f; // Velocidad de movimiento del enemigo
    public float maxBounds = 5f;
    private bool moveRight = true; // Variable para controlar la dirección del movimiento

    Vector2 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        
    }

    private void Update()
    {
        // Mueve el enemigo de izquierda a derecha y viceversa
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        // Calcula el desplazamiento en el eje X
        float horizontalMovement = speed * Time.deltaTime;

        // Determina la dirección del movimiento
        if (moveRight)
        {
            // Mueve hacia la derecha
            transform.Translate(new Vector3(horizontalMovement, 0f, 0f));
        }
        else
        {
            // Mueve hacia la izquierda
            transform.Translate(new Vector3(-horizontalMovement, 0f, 0f));
        }

        // Cambia la dirección cuando el enemigo alcanza ciertos límites
        if (transform.position.x >= initialPosition.x + maxBounds)
        {
            moveRight = false;
        }
        else if (transform.position.x <= initialPosition.x - maxBounds)
        {
            moveRight = true;
        }
    }
    
    // Chris Note: Normally, we check collision in server side. But, in this case, we are using a trigger collider on a client side object.
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
         
            if(!player.isOwned) return;
            
            player.TakeDamage(Vector2.zero, 1);
        }
    }
}
