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
    public float TickRewardValue;

    public void TickReward()
    {
        SetReward(TickRewardValue);
    }

    public void KillTargetReward()
    {
        AddReward(KillTargetRewardValue);
        //Debug.Log("KillTargetReward : " + KillTargetRewardValue);
    }
    public void AttackHitReward()
    {
        AddReward(AttackHitRewardValue);
        //Debug.Log("AttackHitReward : " + AttackHitRewardValue);
    }
    public void AttackMissReward()
    {
        AddReward(AttackMissRewardValue);
        //Debug.Log("AttackMissReward : " + AttackMissRewardValue);
    }
    public void AgentHitReward()
    {
        SetReward(AgentHitRewardValue);
        //Debug.Log("AgentHitReward : " + AgentHitRewardValue);
    }
    public void AgentDieReward()
    {
        AddReward(AgentDieRewardValue);
        //Debug.Log("AgentDieReward : " + AgentDieRewardValue);
    }
}
