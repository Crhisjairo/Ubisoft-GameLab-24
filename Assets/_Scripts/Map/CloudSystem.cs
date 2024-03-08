using System.Collections;
using _Scripts.Shared;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.Map
{
    public class CloudSystem : MonoBehaviour
    {
        [SerializeField] private float minSpawnInterval;
        [SerializeField] private float maxSpawnInterval;

        [SerializeField] private RectTransform _spawnZone;
    

        [SerializeField] private bool startOnAwake;

        private void Start()
        {
            if (startOnAwake)
            {
                StartCoroutine(CreateBackgroundClouds()); 
                StartCoroutine(CreateMiddlegroundCloud()); 
                StartCoroutine(CreateForegroundCloud()); 
            }
        }

        IEnumerator CreateBackgroundClouds()
        {
            while (true)
            {
                float ranSec = CalculateRandomSeconds();
                Vector2 ranPos = CalculateRandomPosition();
                
                ObjectPooler.Instance.SpawnFromPool(MapTags.CloudBackground.ToString(), ranPos, transform.rotation);
                yield return new WaitForSeconds(ranSec);
            }
        }

        private IEnumerator CreateMiddlegroundCloud()
        {
            while (true)
            {
                float ranSec = CalculateRandomSeconds();
                Vector3 ranPos = CalculateRandomPosition();
             
                ObjectPooler.Instance.SpawnFromPool(MapTags.CloudMiddleground.ToString(), ranPos, transform.rotation);
                yield return new WaitForSeconds(ranSec);
            }
        }
    
        IEnumerator CreateForegroundCloud()
        {
            while (true)
            {
                float ranSec = CalculateRandomSeconds();
                Vector3 ranPos = CalculateRandomPosition();
            
                ObjectPooler.Instance.SpawnFromPool(MapTags.CloudForeground.ToString(), ranPos, transform.rotation);
                yield return new WaitForSeconds(ranSec);
            }
        }

        private float CalculateRandomSeconds()
        {
            return Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    
        private Vector3 CalculateRandomPosition()
        {
            float minX = _spawnZone.position.x - _spawnZone.rect.width * 3;
            float maxX = _spawnZone.position.x + _spawnZone.rect.width * 3;
            float minY = _spawnZone.position.y - _spawnZone.rect.height * 3;
            float maxY = _spawnZone.position.y + _spawnZone.rect.height * 3;

            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            return new Vector2(x, y);
        }
    
    }
}
