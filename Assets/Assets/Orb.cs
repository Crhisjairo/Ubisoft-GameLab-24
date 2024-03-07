using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Orb : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite[] sprites;

    public float size = 1f;
    public float minSize = 0.35f;
    public float maxSize = 1.65f;
    public float movementSpeed = 50f;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Assign random properties to make each Orb feel unique
        // spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        // transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);



        // Set the scale and mass of the Orb based on the assigned size so
        // the physics is more realistic
        // transform.localScale = Vector3.one * size;
        // rb.mass = size;
    }
    private void Update()
    {
        // Check if the Y position is under -30 and destroy the Orb
        if (transform.position.y < -30)
        {
            Destroy(gameObject);
        }
    }

    public void SetTrajectory(Vector2 direction)
    {
        // The Orb only needs a force to be added once since they have no
        // drag to make them stop moving
        rb.AddForce(direction * movementSpeed);
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
