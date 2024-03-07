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


    //todo:Patterns  
    //ONCE: triplet: Sword + 2 E-Atk, random position
    //ONCE: triplet: Shield + 2 E-spd, random position
    //diagonal random debuff
    //diagonal inversed random debuff
    //horizontal debuff random

    //Note: randomize Y position


    private void SpawnDiagonal()
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

            // For the first orb, spawn it at a random position
            if (i == 0)
                spawnPoint = GenerateRandomPoint();
            else
                // For subsequent orbs, spawn them diagonally from the previous orb              
                spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y + diagonalOffsetY, 0f);

            previousPosition = spawnPoint;
            Instantiate(RandomDebuff(), spawnPoint, rotation);
        }
    }

    private void SpawnDiagonal_LShape()
    {
        bool inversed = true;
        int spawnAmount = 4;
        Vector3 previousPosition = Vector3.zero;
        float diagonalOffsetX = (inversed ? -1 : 1);
        float diagonalOffsetY = 1;

        for (int i = 0; i < spawnAmount; i++)
        {
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Vector3 spawnPoint;

            // Spawn the first orb randomly
            if (i == 0)
            {
                spawnPoint = GenerateRandomPoint();
            }
            else if (i <= spawnAmount / 2)
            {
                // For the first half of the loop, spawn orbs diagonally upwards
                spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y + diagonalOffsetY, 0f);
            }
            else
            {
                // For the second half of the loop, spawn orbs diagonally downwards
                spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y - diagonalOffsetY, 0f);
            }

            previousPosition = spawnPoint;

            Instantiate(RandomDebuff(), spawnPoint, rotation);
        }
    }

    private void SpawnLShape()
    {
        bool inversed = false;
        int spawnAmount = 4;
        Vector3 previousPosition = Vector3.zero;
        float diagonalOffsetX = (inversed ? -1 : 1);
        float diagonalOffsetY = 1;

        for (int i = 0; i < spawnAmount; i++)
        {
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Vector3 spawnPoint;

            // Spawn the first orb randomly
            if (i == 0)
            {
                spawnPoint = GenerateRandomPoint();
            }
            else if (i <= spawnAmount / 2)
            {
                // For the first half of the loop, spawn orbs diagonally upwards
                spawnPoint = new Vector3(previousPosition.x, previousPosition.y + diagonalOffsetY, 0f);
            }
            else
            {
                // For the second half of the loop, spawn orbs diagonally downwards
                spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y, 0f);
            }

            previousPosition = spawnPoint;

            Instantiate(RandomDebuff(), spawnPoint, rotation);
        }
    }

    private void SingleSpawn()
    {
        for (int i = 0; i < amountPerSpawn; i++)
        {
            // Calculate a random variance in the orb's rotation which will
            // cause its trajectory to change
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            float yVariance = Random.Range(0f, .5f);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            // Calculate the spawn point at the top of the camera's view
            float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1 + yVariance, 0f)).y;
            Vector3 spawnPoint = new Vector3(Random.Range(-spawnDistance, spawnDistance), cameraTopY, 0f);

            orbType = Random.Range(0, 100);
            Instantiate(RandomOrb(), spawnPoint, rotation);
            // Orb orb = Instantiate(RandomDebuff(), spawnPoint, rotation);
            //orb.size = Random.Range(orb.minSize, orb.maxSize);
        }
    }

    private void Spawn()
    {
        SpawnLShape();
    }

    private void SpawnCluster()
    {
        int spawnAmount = 4;
        bool buffCreated = false;
        Vector3 previousPosition = Vector3.zero;
        float minDistance = 1.0f; // Adjust this value based on your requirements

        for (int i = 0; i < spawnAmount; i++)
        {
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            Vector3 spawnPoint;

            if (i == 0)
                spawnPoint = GenerateRandomPoint();

            else
                spawnPoint = generateRandomDiagonalPoint(previousPosition, minDistance);

            previousPosition = spawnPoint;
            int orbType = Random.Range(0, 5);
            Orb orb;

            if (orbType == 0 && buffCreated == false)
            {
                Instantiate(RandomBuff(), spawnPoint, rotation);
                buffCreated = true;
            }
            else if (i == spawnAmount - 1 && buffCreated == false)
            {
                Instantiate(RandomBuff(), spawnPoint, rotation);
            }
            else
            {
                Instantiate(RandomDebuff(), spawnPoint, rotation);
            }

        }
    }

    private Vector3 generateRandomDiagonalPoint(Vector3 previousPosition, float minDistance)
    {
        Vector3 spawnPoint;
        do
        {
            float offsetX = Random.Range(-1f, 1f);
            float offsetY = Random.Range(-1f, 1f);
            spawnPoint = new Vector3(previousPosition.x + offsetX, previousPosition.y + offsetY, 0f);
        } while (Vector3.Distance(spawnPoint, previousPosition) < minDistance);

        return spawnPoint;
    }

    private Vector3 GenerateRandomPoint()
    {
        float yVariance = Random.Range(0f, .5f);
        float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1 + yVariance, 0f)).y;
        return new Vector3(Random.Range(-spawnDistance, spawnDistance), cameraTopY, 0f);
    }
    private Orb RandomOrb()
    {
        orbType = Random.Range(0, 100);
        if (orbType < 60)
            return RandomBuff();
        else
            return RandomDebuff();
    }

    private Orb RandomBuff()
    {
        int debuff = Random.Range(0, 3);
        switch (debuff)
        {
            // case 0: return orbPrefabRed;
            // case 1: return orbPrefabBlue;
            default: return orbPrefabBlue;
        }

    }
    private Orb RandomDebuff()
    {
        int debuff = Random.Range(0, 3);
        switch (debuff)
        {
            // case 0: return orbPrefabRed;
            // case 1: return orbPrefabBlue;
            default: return orbPrefabRed;
        }

    }

}
