using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NonPlayerCharacter : Character
{
    public float moveRadius = 10f;    // NPCê°€ ?´ë™??ë²”ìœ„
    public float moveSpeed = 3.5f;    // ?´ë™ ?ë„
    public float waitTime = 2f;       // ?¤ìŒ ?´ë™ ???€ê¸??œê°„

    private Vector3 targetPosition;   // ëª©í‘œ ?„ì¹˜
    private NavMeshAgent navMeshAgent;       // NavMeshAgent ì»´í¬?ŒíŠ¸

    public override bool Init()
    {
        if (!base.Init()) return false;
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

    IEnumerator MoveToRandomPosition()
    {
        while (true)
        {
            // ?œë¤???„ì¹˜ë¥??ì„±
            targetPosition = GetRandomPosition();

            // NPCë¥??´ë‹¹ ?„ì¹˜ë¡??´ë™
            navMeshAgent.SetDestination(targetPosition);

            // ê²½ë¡œê°€ ? íš¨?œì? ?•ì¸
            yield return new WaitUntil(() => !navMeshAgent.pathPending);

            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid || !navMeshAgent.hasPath)
            {
                //Debug.LogWarning("ê²½ë¡œê°€ ? íš¨?˜ì? ?ŠìŒ. ?¤ë¥¸ ?„ì¹˜ë¥??œë„?©ë‹ˆ??");
                // ê²½ë¡œê°€ ? íš¨?˜ì? ?Šë‹¤ë©? ?¤ì‹œ ?ˆë¡œ???„ì¹˜ë¥??œë„
                continue;
            }

            // NPCê°€ ëª©í‘œ ?„ì¹˜???„ì°©???Œê¹Œì§€ ?€ê¸?
            while (!navMeshAgent.pathPending && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            // ?´ë™ ???€ê¸??œê°„ë§Œí¼ ?€ê¸?
            yield return new WaitForSeconds(waitTime);
        }
    }

    Vector3 GetRandomPosition()
    {
        // NPC???„ì¬ ?„ì¹˜ë¥?ê¸°ì??¼ë¡œ moveRadius ë²”ìœ„ ?´ì˜ ?œë¤ ?„ì¹˜ë¥?ì°¾ìŒ
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, moveRadius, -1);

        return navHit.position;
    }

}
