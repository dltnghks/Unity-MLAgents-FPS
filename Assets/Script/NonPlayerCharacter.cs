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
    private NavMeshAgent navMeshAgent;       // NavMeshAgent 컴포넌트

    public override void Init()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        base.Init();
        Debug.Log("NPC Init");
    }

    public override int AddHP(int val)
    {
        return base.AddHP(val);
    }

    public void OnRandomMove()
    {
        Debug.Log("OnRandomMove");
        StartCoroutine(MoveToRandomPosition());
    }

    IEnumerator MoveToRandomPosition()
    {
        while (true)
        {
            // 랜덤한 위치를 생성
            targetPosition = GetRandomPosition();

            // NPC를 해당 위치로 이동
            navMeshAgent.SetDestination(targetPosition);

            // 경로가 유효한지 확인
            yield return new WaitUntil(() => !navMeshAgent.pathPending);

            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || !navMeshAgent.hasPath)
            {
                //Debug.LogWarning("경로가 유효하지 않음. 다른 위치를 시도합니다.");
                // 경로가 유효하지 않다면, 다시 새로운 위치를 시도
                continue;
            }

            // NPC가 목표 위치에 도착할 때까지 대기
            while (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
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
