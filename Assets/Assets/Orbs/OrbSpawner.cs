﻿using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
namespace Assets.Orbs
{
    public class OrbSpawner : NetworkBehaviour
    {
        public GameObject lifeOrbPrefab;
        public GameObject normalOrbPrefab;
        public GameObject debuffOrbPrefab;

        public GameObject otherOrbPrefab;
        
        bool swordOrb = false;
        bool shieldOrb = false;
        private Camera mainCamera;
        public float spawnDistance = 12f;
        public float spawnRate = 1f;
        public int amountPerSingleSpawnMax = 5;
        [Range(0f, 45f)]
        public float trajectoryVariance = 15f;
        int orbType = 0;

        public bool isDebug = false;
        
        private void Start()
        {
            
            //InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
            mainCamera = Camera.main;
            
            
            if(isServer || isDebug)
                StartCoroutine(Spawn());

        }


        private void SpawnDiagonal(bool inversed)
        {
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
                GameObject newOrb = Instantiate(RandomDebuff(), spawnPoint, Quaternion.Euler(0, 0, 0));
                
                NetworkServer.Spawn(newOrb);
            }
        }


        private void SpawnInverseLShape(bool mirror)
        {
            int spawnAmount = 4;
            Vector3 previousPosition = Vector3.zero;
            Vector3 spawnPoint;
            float offsetX = (mirror ? -1 : 1);
            float offsetY = 1;
            GameObject orb;

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
                GameObject newOrb = Instantiate(orb, spawnPoint, Quaternion.Euler(0, 0, 0));
                
                if(!isDebug)
                    NetworkServer.Spawn(newOrb);
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
            GameObject orb;

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
                GameObject newOrb = Instantiate(orb, spawnPoint, Quaternion.Euler(0, 0, 0));
                
                if(!isDebug)
                    NetworkServer.Spawn(newOrb);
            }
        }

        private void SpawnSingle()
        {
            int spawnAmount = Random.Range(1, amountPerSingleSpawnMax + 1);
            for (int i = 0; i < spawnAmount; i++)
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
                GameObject newOrb = Instantiate(RandomOrb(), spawnPoint, Quaternion.Euler(0, 0, 0));
                
                if(!isDebug)
                    NetworkServer.Spawn(newOrb);
                
                // Orb orb = Instantiate(RandomDebuff(), spawnPoint, rotation);
                //orb.size = Random.Range(orb.minSize, orb.maxSize);
            }
        }

        private IEnumerator Spawn()
        {
            while (true)
            {

                SpawnSingle();
                yield return new WaitForSeconds(1);

                SpawnDiagonal(true);
                yield return new WaitForSeconds(1);

                SpawnDiagonal(false);
                yield return new WaitForSeconds(1);

                SpawnSingle();
                yield return new WaitForSeconds(1);
                SpawnLShape(false);
                yield return new WaitForSeconds(1);

                SpawnInverseLShape(false);
                yield return new WaitForSeconds(1);

                SpawnSingle();
                yield return new WaitForSeconds(1);
                SpawnLShape(true);
                yield return new WaitForSeconds(1);

                SpawnInverseLShape(true);
                yield return new WaitForSeconds(1);
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
        private GameObject RandomOrb()
        {
            orbType = Random.Range(0, 100);
            if (orbType < 85)
                return RandomBuff();
            else
                return RandomDebuff();
        }

        private GameObject RandomBuff()
        {
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0: return lifeOrbPrefab;
                case 1: return normalOrbPrefab;
                default: return otherOrbPrefab;
            }

        }
        private GameObject RandomDebuff()
        {
            return debuffOrbPrefab;
        }

    }
}
