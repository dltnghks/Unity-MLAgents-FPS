using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Controller : Agent
{
    public GameEnvironment environment;
    public GameAgents myAgent;

    [Header("Reward Value")]
    public float KillTargetRewardValue;
    public float AttackHitRewardValue;
    public float AttackMissRewardValue;
    public float AgentHitRewardValue;
    public float AgentDieRewardValue;

    public virtual void KillTargetReward()
    {
    }
    public virtual void AttackHitReward()
    {

    }
    public virtual void AttackMissReward()
    {

    }
    public virtual void AgentHitReward()
    {

    }
    public virtual void AgentDieReward()
    {

    }
}
