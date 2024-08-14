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
    
    public virtual void Clear()
    {
        foreach(var obj in spawnObjectList)
        {
            obj.SetActive(false);
        }
        //spawnObjectList.Clear();
    }

    protected virtual GameObject InstantiateObject()
    {
        if(spawnObjectList.Count > 0)
        {
            foreach(var obj in spawnObjectList)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
        }
        var returnObject = Instantiate(spawnObject);
        spawnObjectList.Add(returnObject);
        returnObject.transform.SetParent(this.transform);
        return returnObject;
    }

    public virtual GameObject OnePointRandomSpawn(int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1) endIndex = spawnPointList.Count;
        var spawnedObject = InstantiateObject();
        int pointIndex = Random.Range(startIndex, endIndex);
        Vector3 position = spawnPointList[pointIndex].transform.localPosition;
        spawnedObject.transform.localPosition = position;
        spawnedObject.transform.localRotation = Quaternion.identity;
        return spawnedObject;
    }

    public GameObject OnePointSpawn(int index)
    {
        var spawnedObject = InstantiateObject();
        Vector3 position = spawnPointList[index].transform.localPosition;
        spawnedObject.transform.localPosition = position;
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
