using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject afterImagePrefab;
    private Queue<GameObject> availableObjets = new Queue<GameObject>();
    public static PlayerAfterImagePool Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjets.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjets.Count == 0)
        {
            GrowPool();
        }
        var instance = availableObjets.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
