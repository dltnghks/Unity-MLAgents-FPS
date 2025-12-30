using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> spawnPointList = new List<GameObject>();
    public GameObject spawnObject;
    public List<GameObject> spawnObjectList = new List<GameObject>();

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
            // obj.SetActive(false);
            if(obj == null)
            {
                continue;
            }
            var character = obj.GetComponent<Character>();
            if(character == null)
            {
                continue;
            }

            character.CharacterSetActive(false);
        }
        //spawnObjectList.Clear();
    }

    public virtual void SpawnObjectListClear()
    {
        foreach (var obj in spawnObjectList)
        {
            Destroy(obj);
        }
        spawnObjectList.Clear();
        //spawnObjectList.Clear();
    }

    protected virtual GameObject InstantiateObject()
    {
        if(spawnObjectList.Count > 0)
        {
            foreach (var obj in spawnObjectList)
            {
                if (obj!= null && !obj.activeSelf)
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

    public List<GameAgents> SpawnTeam(List<GameObject> agentPrefabs, int teamId)
    {
        var spawnedAgents = new List<GameAgents>();
        var availableSpawnPoints = new List<GameObject>(spawnPointList);

        int agentIndex = 0; // Declare agentIndex here
        foreach (var agentPrefab in agentPrefabs)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogWarning("Not enough spawn points for all agents in the team.");
                break;
            }

            var spawnedObject = Instantiate(agentPrefab, transform);
            spawnedObject.name = $"{agentPrefab.name}_Team{teamId}_Agent{agentIndex}"; // New line for naming
            spawnObjectList.Add(spawnedObject);

            int pointIndex = Random.Range(0, availableSpawnPoints.Count);
            GameObject spawnPoint = availableSpawnPoints[pointIndex];
            availableSpawnPoints.RemoveAt(pointIndex);

            spawnedObject.transform.position = spawnPoint.transform.position;
            spawnedObject.transform.rotation = spawnPoint.transform.rotation;

            var gameAgent = spawnedObject.GetComponent<GameAgents>();
            if (gameAgent != null)
            {
                gameAgent.TeamID = teamId;
                spawnedAgents.Add(gameAgent);
            }
            else
            {
                Debug.LogWarning($"The prefab '{agentPrefab.name}' was spawned, but it does not have a GameAgents component attached. It will not be added to the team.", agentPrefab);
            }
            agentIndex++; // Increment agentIndex
        }
        return spawnedAgents;
    }

    public virtual GameObject OnePointRandomSpawn(int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1) endIndex = spawnPointList.Count;
        var spawnedObject = InstantiateObject();
        int pointIndex = Random.Range(startIndex, endIndex);
        Vector3 position = spawnPointList[pointIndex].transform.localPosition;
        spawnedObject.transform.localPosition = position;

        Quaternion rotation = Quaternion.identity;

        switch (pointIndex)
        {
            case 0:
                rotation = Quaternion.Euler(0, 270, 0); // 90 + 180 = 270
                break;
            case 1:
                rotation = Quaternion.Euler(0, 225, 0); // 45 + 180 = 225
                break;
            case 2:
                rotation = Quaternion.Euler(0, 180, 0); // 0 + 180 = 180
                break;
            case 3:
                rotation = Quaternion.Euler(0, 135, 0); // -45 + 180 = 135
                break;
            case 4:
                rotation = Quaternion.Euler(0, 90, 0);  // -90 + 180 = 90
                break;
            case 5:
                rotation = Quaternion.Euler(0, 45, 0);  // -135 + 180 = 45
                break;
            case 6:
                rotation = Quaternion.Euler(0, 0, 0);   // -180 + 180 = 0
                break;
            case 7:
                rotation = Quaternion.Euler(0, -45, 0); // -225 + 180 = -45
                break;
        }
        spawnedObject.transform.localRotation = rotation;
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
