using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NonPlayerCharacter : Character
{
    public float moveRadius = 10f;    // NPC가 ?�동??범위
    public float moveSpeed = 3.5f;    // ?�동 ?�도
    public float waitTime = 2f;       // ?�음 ?�동 ???��??�간

    private Vector3 targetPosition;   // 목표 ?�치
    private NavMeshAgent navMeshAgent;       // NavMeshAgent 컴포?�트

    public bool IsPointMove = false;
    public bool IsTargetMove = false;
    private Transform[] movePoint = new Transform[2];
    private Transform target;

    public override bool Init()
    {
        if (!base.Init()) return false;
        
        IsPointMove = false;
        IsTargetMove = false;
        navMeshAgent = GetComponent<NavMeshAgent>();
        //Debug.Log("NPC Init");
        return true;
    }

    public override int AddHP(int val)
    {
        return base.AddHP(val);
    }

    public void OnRandomMove(float waitTime = 2, float radius = 5, float speed = 8)
    {
        this.waitTime = waitTime;
        this.navMeshAgent.acceleration = speed;
        this.moveRadius = radius;
        //Debug.Log("OnRandomMove");
        StartCoroutine(MoveToRandomPosition());
    }

    public void SetMovePoint(Transform p1, Transform p2)
    {
        IsPointMove = true;
        IsTargetMove = false;

        movePoint[0] = p1;
        movePoint[1] = p2;
    }

    public void SetMoveTarget(Transform _target)
    {
        IsPointMove = false;
        IsTargetMove = true;

        target = _target;
    }

    IEnumerator MoveToRandomPosition()
    {
        bool targetPoint = true;
        while (true)
        {
            if(!GameManager.Instance.IsEpisodeInit)
                yield return null;
            
            // ?�덤???�치�??�성
            if(IsPointMove)
            {
                targetPosition = GetMovePosition(targetPoint ? 1 : 0);
                targetPoint = !targetPoint;
            }
            else if (IsTargetMove)
            {
                targetPosition = target.position;
            }
            else
            {
                targetPosition = GetRandomPosition();
            }

            // NPC�??�당 ?�치�??�동
            navMeshAgent.SetDestination(targetPosition);

            // 경로가 ?�효?��? ?�인
            yield return new WaitUntil(() => !navMeshAgent.pathPending);

            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || !navMeshAgent.hasPath)
            {
                //Debug.LogWarning("경로가 ?�효?��? ?�음. ?�른 ?�치�??�도?�니??");
                // 경로가 ?�효?��? ?�다�? ?�시 ?�로???�치�??�도
                continue;
            }

            // NPC가 목표 ?�치???�착???�까지 ?��?
            while (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            // ?�동 ???��??�간만큼 ?��?
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetRandomPosition()
    {
        // NPC???�재 ?�치�?기�??�로 moveRadius 범위 ?�의 ?�덤 ?�치�?찾음
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, moveRadius, -1);

        return navHit.position;
    }

    private Vector3 GetMovePosition(int targetPoint)
    { 
        Vector3 pos = movePoint[targetPoint].position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(pos, out navHit, moveRadius, -1);

        return navHit.position;
    }

}
