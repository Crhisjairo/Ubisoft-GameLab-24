using System;
using System.Collections;
using _Scripts.Controllers;
using _Scripts.Managers.Multiplayer;
using _Scripts.Map;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Orb : NetworkBehaviour
{
    public float gravityScale = 0.2f;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D collider2D;

    public TextMeshPro amountText;

    public int orbAmount = 1;
    
    [SerializeField]
    private float speed = 5.0f;
    
    [SerializeField] protected int maxOrbAmount = 2;
    [SerializeField] protected int minOrbAmount = 1;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D  = GetComponent<CircleCollider2D>();
    }

    protected virtual void Start()
    {
        amountText.text = orbAmount.ToString();
    }

    public void CalculateRandomAmount()
    {
        orbAmount = Random.Range(minOrbAmount, maxOrbAmount + 1);
        amountText.text = orbAmount.ToString();
    }
    
    private void Update()
    {
        float step = speed * Time.deltaTime;
        transform.Translate(Vector3.down * step);
        
        if(transform.position.y <= -188f)
            Destroy(gameObject);
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
        amountText.enabled = false;
        
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    } 
}
