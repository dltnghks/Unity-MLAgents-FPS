using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class TotalAgents : GameAgents
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
    public float AttackRange = 10;
    public float ShootCoolDown = 0.5f;
    public float ShootTime = 0.0f;
    public float ShootAmount = 30.0f;
    public float ShootCount = 30;

    private Rigidbody _agentRBody;
    private Transform _agentTransform;

    public override void Initialize()
    {
        m_WallJumpSettings = FindObjectOfType<WallJumpSettings>();
        _agentRBody = GetComponent<Rigidbody>();
        _agentTransform = transform;
    }

    public override void OnEpisodeBegin()
    {
        disQueue.Clear();
    }

    private void Update()
    {
        // 플랫폼 바깥으로 떨어지는지 확인
        if (!Physics.Raycast(_agentTransform.position, Vector3.down, 20))
        {
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        // 에이전트의 현재 위치를 상대 좌표로 계산
        var agentPos = _agentRBody.position;// - environment.transform.position;

        // 상대 좌표를 정규화하고 관측 데이터로 추가 3, map scale로 나눠줌
        sensor.AddObservation(agentPos);//  environment.MapSize);

        // Agent rotation 1
        sensor.AddObservation(_agentRBody.transform.eulerAngles.y / 360);

        var localVelocity = _agentTransform.InverseTransformDirection(_agentRBody.velocity);
        // Agent velocity 3
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.y);
        sensor.AddObservation(localVelocity.z);

        sensor.AddObservation(localVelocity);
        // 공격 사거리로 해보기
        //sensor.AddObservation(targetDistance / AttackRange);
        sensor.AddObservation(localVelocity.z);

    }

    public void Jump()
    {
        //finalAgent.bJump = true;
        jumpingTime = 0.3f;
        m_JumpStartingPos = _agentRBody.position;
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

    /// <summary>
    /// Does the ground check.
    /// </summary>
    /// <returns><c>true</c>, if the agent is on the ground,
    /// <c>false</c> otherwise.</returns>
    /// <param name="smallCheck"></param>
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
                if (col != null && col.transform != _agentTransform &&
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
            Physics.Raycast(_agentTransform.position + new Vector3(0, -0.05f, 0), -Vector3.up, out hit,
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

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        ActionSegment<int> act = actionBuffers.DiscreteActions;
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

        _agentRBody.AddForce(dirToGo * m_WallJumpSettings.agentRunSpeed,
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
                new Vector3(_agentRBody.position.x,
                    m_JumpStartingPos.y + m_WallJumpSettings.agentJumpHeight,
                    _agentRBody.position.z) + dirToGo;
            MoveTowards(m_JumpTargetPos, _agentRBody, m_WallJumpSettings.agentJumpVelocity,
                m_WallJumpSettings.agentJumpVelocityMaxChange);
        }
        if (!(jumpingTime > 0f) && !largeGrounded)
        {
            _agentRBody.AddForce(
                Vector3.down * fallingForce, ForceMode.Acceleration);
        }



        jumpingTime -= Time.fixedDeltaTime;


        // search
        var rotateDir = Vector3.zero;
        var rotateDirAction = act[3];

        if (rotateDirAction == 1)
            rotateDir = _agentTransform.up * -1f;
        else if (rotateDirAction == 2)
            rotateDir = _agentTransform.up * 1f;

        _agentTransform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);


        // attack
        var AttackAction = act[4];

        ShootTime -= Time.deltaTime;

        // Rewards
        //AddReward(-1 / MaxStep);
        if (ShootCount > 0 && ShootTime <= 0 && AttackAction == 1)
        {
            ShootTime = ShootCoolDown;
            ShootCount--;

            Debug.DrawRay(_agentRBody.position, _agentTransform.forward * AttackRange, Color.blue);
            RaycastHit hitinfo;
            if (Physics.Raycast(_agentRBody.position, _agentTransform.forward * AttackRange, out hitinfo, AttackRange))
            {
                if (hitinfo.collider.tag == "Target")
                {
                    EndEpisode();
                }
            }
        }

        // Reward  
        AddActionReward();
    }


    protected Queue<float> disQueue = new Queue<float>();
    private bool flag = true;
    // 스텝마다
    public void AddActionReward()
    {

        Vector3 targetDir = (transform.position - transform.position).normalized;
        float Dist = Vector3.Distance(transform.position, transform.position);
        float targetDistance = Dist;


        // 거리관련 reward가 True인 경우에만
        //if (IsDistanceReward)
        {

            RaycastHit hit;
            Physics.Raycast(transform.position + new Vector3(0, -0.05f, 0), -Vector3.up, out hit,
                5f);

            float Dis = Vector3.Distance(transform.position, transform.position);
            disQueue.Enqueue(Dis);

            float preDis;

            if (disQueue.Count > 20)
            {
                if (disQueue.TryPeek(out preDis) && Dis <= preDis - 0.01f && hit.collider && hit.collider.CompareTag("Obstacle"))
                {
                    if (flag)
                    {
                        flag = false;
                    }
                }
                else if (disQueue.TryPeek(out preDis) && Dis <= preDis - 0.01f)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = true;
                }
                disQueue.Dequeue();
            }
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            discreteActionsOut[3] = 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[3] = 2;
        }
        if (ShootTime <= 0.0f)
        {
            if (Input.GetMouseButton(0))
            {
                discreteActionsOut[4] = 1;
            }
        }

        // 점프
        discreteActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
