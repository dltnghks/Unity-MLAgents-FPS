using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : Spawner
{

    public override void Clear()
    {
        foreach (var obj in spawnObjectList)
        {
            obj.GetComponent<Character>().CharacterSetActive(false);
        }
        //spawnObjectList.Clear();
    }

    protected override GameObject InstantiateObject()
    {
        GameObject returnObject = base.InstantiateObject();
        //Debug.Log("NPC Spawn");
        returnObject.GetComponent<Character>().Init();
        return returnObject;

    }

    public void OnEnemyRandomMove(float waitTime = 2, float radius = 5, float speed = 8)
    {
        foreach(var enemy in spawnObjectList)
        {
            enemy.GetComponent<NonPlayerCharacter>().OnRandomMove(waitTime, speed, radius);
        }
    }

    public GameObject PlayerDirectSpawn(Vector3 playerPosition, Vector3 playerDirectionVector, float range = 10f)
    {
        var spawnedObject = InstantiateObject();
        var position = playerPosition + playerDirectionVector * range;
        SetEnemyPosition(spawnedObject, position);
        return spawnedObject;
        //Debug.Log(position);
    }

    public GameObject PlayerCenterRandomSpawn(Vector3 playerPosition, float range = 10f, float start = 0.0f, float end = 2.0f * 2f * Mathf.PI)
    {
        var spawnedObject = InstantiateObject();
        // 0에서 2π 사이의 랜덤 각도를 생성합니다.
        float angle = Random.Range(start, end);

        // 각도에 기반한 랜덤 방향 벡터를 생성합니다.
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        var position = playerPosition + randomDirection*range;
        SetEnemyPosition(spawnedObject, position);
        return spawnedObject;
    }

    public override GameObject OnePointRandomSpawn(int startIndex = 0, int endIndex = -1)
    {
        if (endIndex == -1) endIndex = spawnPointList.Count;
        var spawnedObject = InstantiateObject();
        int pointIndex = Random.Range(startIndex, endIndex);
        Vector3 position = spawnPointList[pointIndex].transform.localPosition;
        SetEnemyPosition(spawnedObject, position);
        return spawnedObject;
    }

    private void SetEnemyPosition(GameObject spawnedObject, Vector3 position)
    {
        spawnedObject.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        spawnedObject.transform.localPosition = position;
        spawnedObject.gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }
}
