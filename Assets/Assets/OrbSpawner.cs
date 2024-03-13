using System.Collections;
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
        //InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
        mainCamera = Camera.main;
        StartCoroutine(Spawn());
    }


    private void SpawnDiagonal()
    {
        bool inversed = false;
        int diagonalSpawnAmount = 4;
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

    //private void SpawnDiagonal_LShape()
    //{
    //    bool inversed = true;
    //    int spawnAmount = 4;
    //    Vector3 previousPosition = Vector3.zero;
    //    float diagonalOffsetX = (inversed ? -1 : 1);
    //    float diagonalOffsetY = 1;

    //    for (int i = 0; i < spawnAmount; i++)
    //    {
    //        float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
    //        Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

    //        Vector3 spawnPoint;

    //        // Spawn the first orb randomly
    //        if (i == 0)
    //        {
    //            spawnPoint = GenerateRandomPoint();
    //        }
    //        else if (i <= spawnAmount / 2)
    //        {
    //            // For the first half of the loop, spawn orbs diagonally upwards
    //            spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y + diagonalOffsetY, 0f);
    //        }
    //        else
    //        {
    //            // For the second half of the loop, spawn orbs diagonally downwards
    //            spawnPoint = new Vector3(previousPosition.x + diagonalOffsetX, previousPosition.y - diagonalOffsetY, 0f);
    //        }

    //        previousPosition = spawnPoint;

    //        Instantiate(RandomDebuff(), spawnPoint, rotation);
    //    }
    //}

    private void SpawnInverseLShape(bool mirror)
    {
        int spawnAmount = 4;
        Vector3 previousPosition = Vector3.zero;
        Vector3 spawnPoint;
        float offsetX = (mirror ? -1 : 1);
        float offsetY = 1;
        Orb orb;

        for (int i = 0; i < spawnAmount; i++)
        {
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);


            // Spawn the first orb randomly
            if (i == 0)
            {
                spawnPoint = GenerateRandomPoint();
                orb = RandomDebuff();
            }
            else if (i <= spawnAmount / 2)
            {
                // For the first half of the loop, spawn orbs diagonally upwards
                spawnPoint = offsetPrevVector(previousPosition, 0, offsetY);
                orb = RandomDebuff();
            }
            else
            {
                spawnPoint = offsetPrevVector(previousPosition, offsetX, 0);
                orb = RandomBuff();
            }

            previousPosition = spawnPoint;
            Instantiate(orb, spawnPoint, rotation);
        }
    }

    private Vector3 offsetPrevVector(Vector3 previous, float offsetX, float offsetY)
    {
        return new Vector3(previous.x + offsetX, previous.y + offsetY, 0f);
    }

    private void SpawnLShape(bool mirror)
    {
        int spawnAmount = 4;
        Vector3 previousPosition = Vector3.zero;
        Vector3 spawnPoint = Vector3.zero;
        float offsetX = (mirror ? -1 : 1);
        float offsetY = 1;
        Orb orb;

        for (int i = 0; i < spawnAmount; i++)
        {
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);


            // Spawn the first orb randomly
            if (i == 0)
            {
                spawnPoint = GenerateRandomPoint();
                orb = RandomBuff();
            }
            else if (i == 1)
            {
                spawnPoint = offsetPrevVector(spawnPoint, offsetX, 0);
                orb = RandomDebuff();
            }
            else
            {
                // For the first half of the loop, spawn orbs diagonally upwards
                spawnPoint = offsetPrevVector(previousPosition, 0, offsetY);
                orb = RandomDebuff();
            }

            previousPosition = spawnPoint;
            Instantiate(orb, spawnPoint, rotation);
        }
    }

    private void SpawnSingle()
    {
        for (int i = 0; i < amountPerSpawn; i++)
        {
            // Calculate a random variance in the orb's rotation which will
            // cause its trajectory to change
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            float yVariance = Random.Range(1f, 1.5f);
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

    private IEnumerator Spawn()
    {
        //SpawnInverseLShape(true);
        //SpawnInverseLShape(false);
        //SpawnLShape(false);
        //SpawnLShape(true);
        while (true)
        {
            SpawnCluster();
            yield return new WaitForSeconds(1);
            SpawnDiagonal();
            yield return new WaitForSeconds(1);
            SpawnSingle();
            yield return new WaitForSeconds(1);
        }
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
        if (orbType < 80)
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
