using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NonPlayerCharacter : Character
{
    public float moveRadius = 10f;    // NPC가 이동할 범위
    public float moveSpeed = 3.5f;    // 이동 속도
    public float waitTime = 2f;       // 다음 이동 전 대기 시간

    private Vector3 targetPosition;   // 목표 위치
    private NavMeshAgent navMeshagent;       // NavMeshAgent 컴포넌트

    public override void Init()
    {
        navMeshagent = GetComponent<NavMeshAgent>();
        base.Init();
    }

    public override int AddHP(int val)
    {
        return base.AddHP(val);
    }

    IEnumerator MoveToRandomPosition()
    {
        while (true)
        {
            // 랜덤한 위치를 생성
            targetPosition = GetRandomPosition();

            // NPC를 해당 위치로 이동
            navMeshagent.SetDestination(targetPosition);

            // NPC가 목표 위치에 도착할 때까지 대기
            while (!navMeshagent.pathPending && navMeshagent.remainingDistance > navMeshagent.stoppingDistance)
            {
                yield return null;
            }

            // 이동 후 대기 시간만큼 대기
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetRandomPosition()
    {
        // NPC의 현재 위치를 기준으로 moveRadius 범위 내의 랜덤 위치를 찾음
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, moveRadius, -1);

        return navHit.position;
    }
}
