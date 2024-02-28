using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Orb : MonoBehaviour
{
    // Collider for the orb
    private SphereCollider orbCollider;
    private float radius = 1.0f;
    private float dropSpeed = 1;
    private const int MAXSPEED=3,MINSPEED=1;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        // Add a SphereCollider component
        orbCollider = gameObject.AddComponent<SphereCollider>();
        orbCollider.radius = radius;
        mainCamera = Camera.main; ;
        speedRandomizer();
    }

    // Update is called once per frame
    void Update()
    {
        MoveOrb();
    }

    public float getRadius()
    {
        return radius;
    }

    //todo:temporarily an int since Player class not added
    public virtual void Interact(int player)
    {
        Destroy(gameObject);
    }
    //todo: Select a max speed later on
    private void speedRandomizer()
    {
        dropSpeed = Random.Range(MINSPEED, MAXSPEED);
    }

    //public void TakeDamage(int damageAmount)
    //{
    //    orbHealth -= damageAmount;
    //    if (orbHealth <= 0)
    //        Destroy(gameObject);
    //}
    void MoveOrb()
    {
        // Calculate the new position based on the drop speed
        Vector3 newPosition = transform.position - Vector3.up * dropSpeed * Time.deltaTime;

        if (newPosition.y < GetScreenBottom())
            Destroy(gameObject);
        else
            transform.position = newPosition;
    }
    float GetScreenBottom()
    {
        // Get the camera's position in world space
        Vector3 cameraPosition = mainCamera.transform.position;

        // Calculate the bottom of the screen in world space
        float screenBottom = cameraPosition.y - mainCamera.orthographicSize;
        return screenBottom;
    }
}

public class RedOrb : Orb
{

    public override void Interact(int player)
    {
        // Add specific behavior for Red Orb
        //player.RestoreHealth(healthPoints);

        // Call the base class implementation for common logic
        base.Interact(player);
    }
}

public class BlueOrb : Orb
{
    public override void Interact(int player)
    {
        base.Interact(player);
    }
}

public class BlueNerfOrb : Orb
{
    public override void Interact(int player)
    {
        base.Interact(player);
    }
}



public class OrbFactory
{
    // numerical value of orb types that can be created
    private int numOfOrbTypes = 3;
    private Camera mainCamera;
    private float orbRadius;
    private Orb orb = new Orb();

    void Start()
    {
        mainCamera = Camera.main; ;
        orbRadius= orb.getRadius();
    }

    //todo: test comment code
    public Orb CreateOrb()
    {
        Orb orby = randomizer();
        orby.transform.position = RandomStartPosition();

        // Instantiate the selected orb type
        //Orb newOrb = (Orb)gameObject.AddComponent(selectedOrbType);

        return orby;
    }


    Vector3 RandomStartPosition()
    {
        float screenWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float randomX = Random.Range(-screenWidth, screenWidth);

        float topOfScreen = mainCamera.transform.position.y + mainCamera.orthographicSize;
        float randomY = topOfScreen + orbRadius;

        // Set the orb's position
        return new Vector3(randomX, randomY, 0f);
    }

 

    private Orb randomizer()
    {
        int randomIndex = Random.Range(0, numOfOrbTypes);
        switch (randomIndex)
        {
            case 0: return new RedOrb();
            case 1: return new BlueNerfOrb();
            default: return new BlueOrb();
        }
        
    }
}