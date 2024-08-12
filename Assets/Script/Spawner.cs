using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> spawnPointList = new List<GameObject>();
    public GameObject spawnObject;
    protected List<GameObject> spawnObjectList = new List<GameObject>();

    private void Awake()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            spawnPointList.Add(transform.GetChild(i).gameObject);
        }
    }

    public void Clear()
    {
        foreach(var obj in spawnObjectList)
        {
            Destroy(obj);
        }
        spawnObjectList.Clear();
    }

    protected virtual GameObject InstantiateObject()
    {
        var returnObject = Instantiate(spawnObject);
        spawnObjectList.Add(returnObject);
        returnObject.transform.SetParent(this.transform);
        return returnObject;
    }

    public GameObject OnePointRandomSpawn()
    {
        var spawnedObject = InstantiateObject();
        int pointIndex = Random.Range(0, spawnPointList.Count);
        Vector3 position = spawnPointList[pointIndex].transform.localPosition;
        spawnedObject.transform.localPosition = position;
        spawnedObject.transform.localRotation = Quaternion.identity;
        return spawnedObject;
    }

    public void OnePointSpawn(Vector3 position, Quaternion rotation)
    {
        var spawnedObject = InstantiateObject();
        spawnedObject.transform.SetPositionAndRotation(position, rotation);
    }

    public void AllPointSpawn()
    {
        foreach(var point in spawnPointList)
        {
            var spawnedObject = InstantiateObject();
            Vector3 position = point.transform.localPosition;
            spawnedObject.transform.localPosition = position;
            //spawnedObject.transform.rotation = Quaternion.identity;
        }
    }

}
