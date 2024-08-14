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

    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }

    public void Init(GameEnvironment environment)
    {
        if (!base.Init()) return;

        Debug.Log("Agent Init");
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
        var smallGrounded = DoGroundCheck(true);
        var largeGrounded = DoGroundCheck(false);

        var dirToGo = Vector3.zero;
        var dirToGoForwardAction = act[0];
        var dirToGoSideAction = act[1];
        var jumpAction = act[2];

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

        rBody.AddForce(dirToGo * m_WallJumpSettings.agentRunSpeed,
        ForceMode.VelocityChange);

        if (jumpAction == 1)
        {
            if ((jumpingTime <= 0f) && smallGrounded)
            {
                Jump();
            }
        }

        if (jumpingTime > 0f)
        {
            m_JumpTargetPos =
                new Vector3(rBody.position.x,
                    m_JumpStartingPos.y + m_WallJumpSettings.agentJumpHeight,
                    rBody.position.z) + dirToGo;
            MoveTowards(m_JumpTargetPos, rBody, m_WallJumpSettings.agentJumpVelocity,
                m_WallJumpSettings.agentJumpVelocityMaxChange);
        }
        if (!(jumpingTime > 0f) && !largeGrounded)
        {
            rBody.AddForce(
                Vector3.down * fallingForce, ForceMode.Acceleration);
        }

        jumpingTime -= Time.fixedDeltaTime;


        // search
        var rotateDir = Vector3.zero;
        var rotateDirAction = act[3];

        if (rotateDirAction == 1)
            rotateDir = transform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = transform.up * 1f;

        transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);

    }


    public void AttackAction(int act)
    {
        // attack
        var AttackAction = act;

        ShootTime -= Time.deltaTime;
        if (ShootCount > 0 && ShootTime <= 0 && AttackAction == 1)
        {
            Debug.Log("Attack");
            ShootTime = ShootCoolDown;
            ShootCount--;

            StartCoroutine(AttackDelay());
            Debug.DrawRay(rBody.position, transform.forward * AttackRange, Color.blue);
            RaycastHit hitinfo;
            if (Physics.Raycast(rBody.position, transform.forward, out hitinfo, AttackRange))
            {
                if (hitinfo.collider.tag == "Target" || (hitinfo.collider.tag == "Player" && hitinfo.collider.gameObject.GetComponent<Character>().TeamID != this.TeamID))
                {
                    //Debug.Log("Hit");
                    AddReward(ERewardType.AttackHit);
                    if (0 >= hitinfo.collider.gameObject.GetComponent<Character>().AddHP(-AttackDamage))
                    {
                        AddReward(ERewardType.KillTarget);
                        GameManager.GameClear(environment);
                    }
                }
            }
            else
            {
                //Debug.Log("Miss");
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
        //finalAgent.bJump = true;
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
                    grounded = true; //then we're grounded
                    break;
                }
            }
            return grounded;
        }
        else
        {
            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, -0.05f, 0), -Vector3.up, out hit,
                1f);

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
        if (curHP <= 0)
        {
            AddReward(ERewardType.AgentDie);
        }
        if (val > 0)
        {
            AddReward(ERewardType.AgentHit);
        }
        return curHP;
    }
}
