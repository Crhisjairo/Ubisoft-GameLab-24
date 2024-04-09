using System;
using System.Collections;
using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer;
using _Scripts.Map;
using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Orb : NetworkBehaviour
{
    public float gravityScale = 0.2f;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D collider2D;

    public TextMeshPro amount;

    [SerializeField]
    private float speed = 5.0f;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D  = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        float step = speed * Time.deltaTime;
        transform.Translate(Vector3.down * step);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerServerDataSync = null;
        
        if (other.CompareTag("Player"))
        {
            playerServerDataSync = other.GetComponent<PlayerController>();
            //TODO: in boss fight ?
        } else if (other.CompareTag("Basket"))
        {
            // NOTE: Only the basket has an specific behavior when colliding with an orb.
            //Basket basket = other.GetComponent<Basket>();
            playerServerDataSync = NetworkClient.connection.identity.GetComponent<PlayerController>();
            
        }
        
        if (playerServerDataSync != null)
        {
            OnApplyOrbEffect(playerServerDataSync);
            StartCoroutine(DestroyDelayed());
        }
        
    }

    public virtual void OnApplyOrbEffect(PlayerController playerController)
    {
        Debug.LogWarning("Orb effect not implemented in " + gameObject.name + ", please override OnApplyOrbEffect method.");
        Debug.LogWarning("Collision with -> PlayerServerDataSync: " + playerController.gameObject.name);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    private IEnumerator DestroyDelayed()
    {
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
        amount.enabled = false;
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    } 
}
