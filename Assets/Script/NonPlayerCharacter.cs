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
    private NavMeshAgent navMeshagent;       // NavMeshAgent ������Ʈ

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
            // ������ ��ġ�� ����
            targetPosition = GetRandomPosition();

            // NPC�� �ش� ��ġ�� �̵�
            navMeshagent.SetDestination(targetPosition);

            // NPC�� ��ǥ ��ġ�� ������ ������ ���
            while (!navMeshagent.pathPending && navMeshagent.remainingDistance > navMeshagent.stoppingDistance)
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
