using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class GameAgents : Player
{
    protected WallJumpSettings m_WallJumpSettings;
    [Header("JumpSetting")]
    public float jumpingTime;
    public float jumpTime;
    // This is a downward force applied when falling to make jumps look
    // less floaty
    public float fallingForce;
    // Use to check the coliding objects
    public Collider[] hitGroundColliders = new Collider[4];
    protected Vector3 m_JumpTargetPos;
    protected Vector3 m_JumpStartingPos;


    [Header("AttackSetting")]
    public int AttackDamage = 25;
    public float AttackRange = 30;
    public float ShootCoolDown = 0.5f;
    public float ShootTime = 0.0f;
    public float ShootAmount = 30.0f;
    public float ShootCount = 30;

    public GameEnvironment environment;
    public Rigidbody rBody;
    public Vector3 targetDir;
    public float targetDistance;
    public GameObject AttackObject;
    public GameObject target; // This is a reference to the actual GameObject of the target

    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }

    private bool _isSetting = false;
    public void Init(GameEnvironment environment)
    {
        if (!base.Init()) return;

        if(AttackObject)
            AttackObject.SetActive(false);
        ShootCount = ShootAmount;

        if (_isSetting) return;

        _isSetting = true;
        //Debug.Log("Agent Init");
        AttackObject = transform.GetChild(0).gameObject;
        float attackObjectScale = AttackRange;
        AttackObject.transform.localScale = new Vector3(0.1f, 0.1f, attackObjectScale);
        AttackObject.transform.localPosition = new Vector3(0, 0, attackObjectScale / 2);
        AttackObject.SetActive(false);
        this.environment = environment;
        m_WallJumpSettings = FindObjectOfType<WallJumpSettings>();
        rBody = GetComponent<Rigidbody>();
        var controllerList = GetComponentsInChildren<Controller>();
        foreach (var controller in controllerList)
        {
            controller.myAgent = this;
            controller.environment = environment;
            _controllerList.Add(controller);
        }
    }

    public void MovementAction(int[] act)
    {
        if (!GameManager.Instance.IsEpisodeInit)
        {
            return;
        }

        if (!_initialized)
        {
            return;
        }
        var smallGrounded = DoGroundCheck(true);
        var largeGrounded = DoGroundCheck(false);

        var dirToGo = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var dirToGoSideAction = act[1];
        //var jumpAction = act[2];

        if (dirToGoForwardAction == 1)
        {
            dirToGo += 1f * transform.forward;
        }
        if (dirToGoForwardAction == 2)
        {
            dirToGo += -1f * transform.forward;
        }
        if (dirToGoSideAction == 1)
        {
            dirToGo += 1f * transform.right;
        }
        if (dirToGoSideAction == 2)
        {
            dirToGo += -1f * transform.right;
        }

        rBody.velocity = dirToGo * m_WallJumpSettings.agentRunSpeed;
        
        var rotateDir = Vector3.zero;
        var rotateDirAction = act[2];

        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 7.5f * m_WallJumpSettings.agentRunSpeed);
    }

    public void FixedUpdate()
    {
        if (!_initialized)
        {
            return;
        }

        GameObject currentTarget = null;

        if (TestManager.Instance != null && (TestManager.Instance.IsTest || TestManager.Instance.IsEnemy))
        {
            List<GameAgents> opposingTeamAgents = null;
            if (TeamID == 0)
            {
                opposingTeamAgents = environment.team2Agents;
            }
            else // TeamID == 1
            {
                opposingTeamAgents = environment.team1Agents;
            }

            if (opposingTeamAgents != null)
            {
                foreach (var agent in opposingTeamAgents)
                {
                    if (agent != null && agent.gameObject.activeSelf && agent.HP > 0)
                    {
                        currentTarget = agent.gameObject;
                        break;
                    }
                }
            }
        }
        else 
        {
            if (GameManager.GamePhase < 8)
            {
                if (environment.Enemy != null)
                {
                    currentTarget = environment.Enemy.gameObject;
                }
            }
            else 
            {
                if (TeamID == 0 && environment.team2Agents.Count > 0 && environment.team2Agents[0] != null)
                {
                    currentTarget = environment.team2Agents[0].gameObject;
                }
                else if (TeamID == 1 && environment.team1Agents.Count > 0 && environment.team1Agents[0] != null)
                {
                    currentTarget = environment.team1Agents[0].gameObject;
                }
            }
        }

        if (currentTarget != null)
        {
            target = currentTarget;
            targetDir = (target.transform.position - transform.position).normalized;
            targetDistance = AttackRange / Vector3.Distance(transform.position, target.transform.position);
            AddReward(ERewardType.Tick);
            RaycastHit hit;
            if (Vector3.Angle(transform.forward, targetDir) < 15.0f
                && Physics.Raycast(transform.position, targetDir, out hit, targetDistance * AttackRange))
            {
                if (hit.collider.gameObject == target)
                {
                    AddReward(ERewardType.SeeingEnemy);
                }
            }
        }
        else
        {
            AddReward(ERewardType.SeeingEnemy);
        }
    }

    public void AttackAction(int act)
    {
        if (!_initialized)
        {
            return;
        }
        
        var AttackAction = act;

        ShootTime -= Time.deltaTime;
        if (ShootCount > 0 && ShootTime <= 0 && AttackAction == 1)
        {
            ShootTime = ShootCoolDown;
            ShootCount--;

            RaycastHit hitinfo;
            bool hasHit = Physics.Raycast(rBody.position, transform.forward, out hitinfo, AttackRange);

            if (hasHit && (hitinfo.collider.tag == "Target" || (hitinfo.collider.tag == "Player" && hitinfo.collider.gameObject.GetComponent<Character>().TeamID != this.TeamID)))
            {
                // My attack was successful, so increment my AttackCount
                AddReward(ERewardType.AttackHit);

                var victimAgent = hitinfo.collider.gameObject.GetComponent<GameAgents>();
                if (victimAgent != null)
                {
                    // Notify the victim that it was hit, so it can increment its HitCount
                    victimAgent.AddReward(ERewardType.AgentHit);
                }

                // Damage the target and check for kill
                if (0 >= hitinfo.collider.gameObject.GetComponent<Character>().AddHP(-AttackDamage))
                {
                    // I killed the target, so increment my KillCount
                    AddReward(ERewardType.KillTarget);
                }
            }
            else
            {
                // My attack missed
                AddReward(ERewardType.AttackMiss);
            }
        }
    }

    IEnumerator AttackDelay()
    {
        AttackObject.SetActive(true);
        yield return new WaitForSeconds(Time.deltaTime);
        AttackObject.SetActive(false);
    }

    public void Jump()
    {
        jumpingTime = 0.3f;
        m_JumpStartingPos = rBody.position;
    }

    protected void MoveTowards(Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
    {
        var moveToPos = targetPos - rb.worldCenterOfMass;
        var velocityTarget = Time.fixedDeltaTime * targetVel * moveToPos;
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(
                rb.velocity, velocityTarget, maxVel);
        }
    }

    public bool DoGroundCheck(bool smallCheck)
    {
        if (!smallCheck)
        {
            hitGroundColliders = new Collider[4];
            var o = this;
            Physics.OverlapBoxNonAlloc(
                o.transform.position + new Vector3(0, -0.05f, 0),
                new Vector3(0.95f / 2f, 0.5f, 0.95f / 2f),
                hitGroundColliders,
                o.transform.rotation);
            var grounded = false;
            foreach (var col in hitGroundColliders)
            {
                if (col != null && col.transform != transform &&
                    (col.CompareTag("WalkableSurface") ||
                     col.CompareTag("Obstacle") ||
                     col.CompareTag("Wall")))
                {
                    grounded = true; 
                    break;
                }
            }
            return grounded;
        }
        else
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, -0.05f, 0), -Vector3.up, out hit, 1f);

            if (hit.collider != null &&
                (hit.collider.CompareTag("WalkableSurface") ||
                 hit.collider.CompareTag("Obstacle") ||
                 hit.collider.CompareTag("Wall"))
                && hit.normal.y > 0.95f)
            {
                return true;
            }
            return false;
        }
    }

    public override int AddHP(int val)
    {
        int curHP = base.AddHP(val);
        if (curHP <= 0 && gameObject.activeSelf) // ensure it only happens once
        {
            // I died, so increment my DeathCount
            AddReward(ERewardType.AgentDie); 
            CharacterSetActive(false); 
            environment.OnAgentDied(this);
        }
        return curHP;
    }
}