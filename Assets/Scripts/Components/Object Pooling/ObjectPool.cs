using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    private class Pool {
        public VarString tag;
        public int size;
        public GameObject prefab;
        public Transform defaultParent;
        private Queue<GameObject> objectQueue = new();

        public void Initialize() {
            for (int i = 0;i < size;i++) {
                GameObject go = Instantiate(prefab, defaultParent);
                objectQueue.Enqueue(go);
                go.SetActive(false);
            }
        }

        public GameObject ActivateObject(Vector3 position, Quaternion rotation, Transform parentTransform, bool keepWorldPosition) {
            for (int i = 0;i < size;i++) {
                GameObject obj = objectQueue.Dequeue();
                if (obj != null)
                {
                    objectQueue.Enqueue(obj);
                    if (obj.activeInHierarchy)
                    {
                        continue;
                    }
                    Transform t = obj.GetComponent<Transform>();
                    t.SetParent(parentTransform, keepWorldPosition);
                    t.SetPositionAndRotation(position, rotation);
                    obj.SetActive(true);
                    return obj;
                }
                else {
                    // Replenish destroyed objects
                    GameObject oj = Instantiate(prefab, position, rotation);
                    objectQueue.Enqueue(oj);
                    oj.transform.SetParent(parentTransform, keepWorldPosition);
                    oj.SetActive(true);
                    return oj;
                }
            }
            // Increase the size of pool if no available object can be found
            size++;
            GameObject o = Instantiate(prefab, position, rotation);
            objectQueue.Enqueue(o);
            o.SetActive(true);
            return o;
        }
    }
    private static ObjectPool instance;
    [SerializeField] private Pool[] pools;
    private Dictionary<string, Pool> poolz = new();

    public static ObjectPool Instance {
        get { return instance; }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { 
            Destroy(gameObject);
            return;
        }
        foreach (Pool pool in pools) {
            pool.Initialize();
            poolz[pool.tag.Value] = pool;
        }
    }

    public GameObject GetObject(string tag, Vector3 position, Quaternion rotation, Transform parentTransform = null, bool keepWorldPosition = false) {
        if (poolz.ContainsKey(tag)) {
            return poolz[tag].ActivateObject(position, rotation, parentTransform, keepWorldPosition);
        }
        return null;
    }
}
