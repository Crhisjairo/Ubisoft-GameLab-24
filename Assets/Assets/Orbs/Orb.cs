using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Orb : MonoBehaviour
{
    private Rigidbody2D rb;
    public float gravityScale = 0.2f;
    private SpriteRenderer spriteRenderer;

    [SerializeField]


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = gravityScale;
    }

    private void Start()
    {
        //transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);
    }
    private void Update()
    {
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }


    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    // if (collision.gameObject.CompareTag("Basket"))
    // {
    //      triggerOrbEffect()
    //     GameManager.Instance.OnOrbDestroyed(this);
    //     Destroy(gameObject);
    // }
    //}

    //public virtual void triggerOrbEffect(){}

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.CompareTag("Player"))
        // {
        //     PlayerController player = other.GetComponent<PlayerController>();
        //     player.AddHealth(1);
        // }
    }

}
