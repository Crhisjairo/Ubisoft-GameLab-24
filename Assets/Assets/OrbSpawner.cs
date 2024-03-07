using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public Orb orbPrefabRed;
    bool swordOrb = false;
    bool shieldOrb = false;
    public Orb orbPrefabBlue;
    private Camera mainCamera;
    public float spawnDistance = 12f;
    public float spawnRate = 1f;
    public int amountPerSpawn = 1;
    [Range(0f, 45f)]
    public float trajectoryVariance = 15f;
    int orbType = 0;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
        mainCamera = Camera.main;
        // Debug.LogWarning("WRONG SETUP OF SPWAN RATE");
    }
    private void update()
    {

    }

    // public void Spawn()
    // {
    //     for (int i = 0; i < amountPerSpawn; i++)
    //     {
    //         // Calculate a random variance in the orb's rotation which will
    //         // cause its trajectory to change
    //         float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
    //         float yVariance = Random.Range(0f, .5f);
    //         Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

    //         // Calculate the spawn point at the top of the camera's view
    //         float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1 + yVariance, 0f)).y;
    //         Vector3 spawnPoint = new Vector3(Random.Range(-spawnDistance, spawnDistance), cameraTopY, 0f);

    //         // Create the new orb by cloning the prefab and set a random
    //         // size within the range
    //         Orb orb;
    //         orbType = Random.Range(0, 100);
    //         if (orbType < 40)
    //             orb = Instantiate(orbPrefabRed, spawnPoint, rotation);
    //         else
    //             orb = Instantiate(orbPrefabBlue, spawnPoint, rotation);

    //         orb.size = Random.Range(orb.minSize, orb.maxSize);
    //     }
    // }

    void orbRandomizer(int orbType, Vector3 spawnPoint, Quaternion rotation)
    {
        Orb orb;
        if (orbType > 90 && !swordOrb)
        {
            swordOrb = false;
            orb = Instantiate(orbPrefabRed, spawnPoint, rotation);
        }
        else if (orbType > 80 && !shieldOrb)
        {
            shieldOrb = false;
            orb = Instantiate(orbPrefabRed, spawnPoint, rotation);
        }
        else if (orbType > 70)
        {
            //DIAGONAL
        }
        else if (orbType > 60)
        {
            //DIAGONAL INVERSED
        }
        else if (orbType > 50)
        {
            //HORIZONTAL
        }
        else if (orbType > 40)
        {
            //debuff orb
        }
        orb = Instantiate(orbPrefabBlue, spawnPoint, rotation);

    }
    void triangularSpawn()
    {
        //
    }


    //todo:Patterns  
    //ONCE: triplet: Sword + 2 E-Atk, random position
    //ONCE: triplet: Shield + 2 E-spd, random position
    //diagonal random debuff
    //diagonal inversed random debuff
    //horizontal debuff random

    //Note: randomize Y position

    public void Spawn()
    {
        bool inversed = false;
        int diagonalSpawnAmount = 5;
        Vector3 previousPosition = new Vector3(0, 0, 0f);
        float diagonalOffsetX = (inversed ? -1 : 1);
        float diagonalOffsetY = 1;

        for (int i = 0; i < diagonalSpawnAmount; i++)
        {
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);

            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Vector3 spawnPoint;
            Orb orb;

            // For the first orb, spawn it at a random position
            if (i == 0)
                spawnPoint = GenerateRandomSpawnPoint();
            else
                // For subsequent orbs, spawn them diagonally from the previous orb              
                spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y + diagonalOffsetY, 0f);
            
            previousPosition = spawnPoint;

            // Instantiate orb based on the random orbType
            orbType = Random.Range(0, 5);
            if (orbType < 3)
                orb = Instantiate(orbPrefabRed, spawnPoint, rotation);
            else
                orb = Instantiate(orbPrefabBlue, spawnPoint, rotation);

            orb.size = Random.Range(orb.minSize, orb.maxSize);
        }
    }

    private Vector3 GenerateRandomSpawnPoint()
    {
        float yVariance = Random.Range(0f, .5f);
        float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1 + yVariance, 0f)).y;
        return new Vector3(Random.Range(-spawnDistance, spawnDistance), cameraTopY, 0f);
    }


}
