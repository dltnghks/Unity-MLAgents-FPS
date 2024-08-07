using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> spawnPointList = new List<GameObject>();
    public GameObject spawnObject;
    private List<GameObject> spawnObjectList = new List<GameObject>();

    private void Start()
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

    protected GameObject InstantiateObject()
    {
        var returnObject = Instantiate(spawnObject);
        spawnObjectList.Add(returnObject);
        returnObject.transform.SetParent(this.transform);
        return returnObject;
    }

    public void OnePointRandomSpawn()
    {
        var spawnedObject = InstantiateObject();
        int pointIndex = Random.Range(0, spawnPointList.Count);
        Vector3 position = spawnPointList[pointIndex].transform.position;
        spawnedObject.transform.position = position;
        spawnedObject.transform.rotation = Quaternion.identity;
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
            Vector3 position = point.transform.position;
            spawnedObject.transform.position = position;
            //spawnedObject.transform.rotation = Quaternion.identity;
        }
    }

    public void PlayerDirectSpawn(Vector3 playerPosition, Vector3 playerDirectionVector)
    {
        var spawnedObject = InstantiateObject();
        var position = playerPosition + playerDirectionVector * 10f;
        spawnedObject.transform.position = position;
    }
}
