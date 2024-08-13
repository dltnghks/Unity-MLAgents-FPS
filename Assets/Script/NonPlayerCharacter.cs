using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NonPlayerCharacter : Character
{
    public float moveRadius = 10f;    // NPC�� �̵��� ����
    public float moveSpeed = 3.5f;    // �̵� �ӵ�
    public float waitTime = 2f;       // ���� �̵� �� ��� �ð�

    private Vector3 targetPosition;   // ��ǥ ��ġ
    private NavMeshAgent navMeshAgent;       // NavMeshAgent ������Ʈ

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
            // ������ ��ġ�� ����
            targetPosition = GetRandomPosition();

            // NPC�� �ش� ��ġ�� �̵�
            navMeshAgent.SetDestination(targetPosition);

            // ��ΰ� ��ȿ���� Ȯ��
            yield return new WaitUntil(() => !navMeshAgent.pathPending);

            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || !navMeshAgent.hasPath)
            {
                //Debug.LogWarning("��ΰ� ��ȿ���� ����. �ٸ� ��ġ�� �õ��մϴ�.");
                // ��ΰ� ��ȿ���� �ʴٸ�, �ٽ� ���ο� ��ġ�� �õ�
                continue;
            }

            // NPC�� ��ǥ ��ġ�� ������ ������ ���
            while (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            // �̵� �� ��� �ð���ŭ ���
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetRandomPosition()
    {
        // NPC�� ���� ��ġ�� �������� moveRadius ���� ���� ���� ��ġ�� ã��
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, moveRadius, -1);

        return navHit.position;
    }

}
