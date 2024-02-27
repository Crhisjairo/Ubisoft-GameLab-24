using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Orb : MonoBehaviour
{
    // Collider for the orb
    private SphereCollider orbCollider;
    private float orbRadius = 1.0f;
    private float dropSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Add a SphereCollider component
        orbCollider = gameObject.AddComponent<SphereCollider>();
        orbCollider.radius = orbRadius;
        speedRandomizer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //todo:temporarily an int since Player class not added
    public virtual void Interact(int player)
    {
        Destroy(gameObject);
    }
    private void speedRandomizer()
    {
        dropSpeed = 2;
    }

    //public void TakeDamage(int damageAmount)
    //{
    //    orbHealth -= damageAmount;
    //    if (orbHealth <= 0)
    //        Destroy(gameObject);
    //}
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

    public Orb CreateOrb(Vector3 position)
    {
        Orb orby = randomizer();
        orby.transform.position = position;

        // Instantiate the selected orb type
        //Orb newOrb = (Orb)gameObject.AddComponent(selectedOrbType);

        return orby;
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