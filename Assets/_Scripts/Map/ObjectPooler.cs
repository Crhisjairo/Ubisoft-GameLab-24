using System.Collections.Generic;
using _Scripts.Interfaces;
using UnityEngine;
namespace _Scripts.Map
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int maxActiveObjects;
        }
    
        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDict;

        public static ObjectPooler Instance;

        private void Awake()
        {
            poolDict = new Dictionary<string, Queue<GameObject>>();
            Instance = this;
        }
        
        void Start()
        {

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.maxActiveObjects; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
            
                poolDict.Add(pool.tag, objectPool);
            }
        }
    
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDict.ContainsKey(tag))
            {
                //Debug.Log("El Pool con el tag " + tag + " no existe.");
                return null;
            }
        
            GameObject objToSpawn = poolDict[tag].Dequeue();
        
            objToSpawn.SetActive(true);
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;

            IPooledObject pooledObject = objToSpawn.GetComponent<IPooledObject>();

            if (pooledObject != null)
            {
                pooledObject.OnObjectSpawn();
            }
        
            poolDict[tag].Enqueue(objToSpawn);

            return objToSpawn;
        }
    
    
    }
}
