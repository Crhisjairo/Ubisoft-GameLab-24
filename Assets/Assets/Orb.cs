using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Orb : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField]


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);
    }
    private void Update()
    {
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Basket"))
        // {
        //      triggerOrbEffect()
        //     GameManager.Instance.OnOrbDestroyed(this);
        //     Destroy(gameObject);
        // }
    }


    public virtual void triggerOrbEffect()
    {
        //
    }


}
