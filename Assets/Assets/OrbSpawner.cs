using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    public Orb orbPrefabRed;
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
    }
    private void update()
    {

    }

    public void Spawn()
    {
        for (int i = 0; i < amountPerSpawn; i++)
        {
            // Calculate a random variance in the orb's rotation which will
            // cause its trajectory to change
            float variance = Random.Range(-trajectoryVariance, trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward);

            // Calculate the spawn point at the top of the camera's view
            float cameraTopY = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0f)).y;
            Vector3 spawnPoint = new Vector3(Random.Range(-spawnDistance, spawnDistance), cameraTopY, 0f);

            // Create the new orb by cloning the prefab and set a random
            // size within the range
            Orb orb;
            orbType = Random.Range(0, 10);
            if (orbType < 4)
            {
                orb = Instantiate(orbPrefabRed, spawnPoint, rotation);
            }
            else
            {
                orb = Instantiate(orbPrefabBlue, spawnPoint, rotation);
            }


            orb.size = Random.Range(orb.minSize, orb.maxSize);
        }
    }

}
