using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : Spawner
{
    protected override GameObject InstantiateObject()
    {
        GameObject returnObject = base.InstantiateObject();
        Debug.Log("NPC Spawn");
        returnObject.GetComponent<Character>().Init();
        return returnObject;

    }
    public void PlayerDirectSpawn(Vector3 playerPosition, Vector3 playerDirectionVector, float range = 10f)
    {
        var spawnedObject = InstantiateObject();
        var position = playerPosition + playerDirectionVector * range;
        spawnedObject.gameObject.GetComponent<NavMeshAgent>().enabled = false;
        spawnedObject.transform.localPosition = position;
        spawnedObject.gameObject.GetComponent<NavMeshAgent>().enabled = true;
        Debug.Log(position);
    }

    public void PlayerCenterRandomSpawn(Vector3 playerPosition, float range = 10f)
    {
        var spawnedObject = InstantiateObject();
        // 0에서 2π 사이의 랜덤 각도를 생성합니다.
        float angle = Random.Range(0f, 2f * Mathf.PI);

        // 각도에 기반한 랜덤 방향 벡터를 생성합니다.
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        var position = playerPosition + randomDirection*range;
        spawnedObject.transform.localPosition = position;
    }
}
