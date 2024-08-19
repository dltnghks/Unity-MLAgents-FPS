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
    public float SeeingEnemyRewardValue;

    public void TickReward()
    {
        SetReward(TickRewardValue);
    }

    public void KillTargetReward()
    {
        AddReward(KillTargetRewardValue);
        //Debug.Log(name + ", KillTargetReward : " + KillTargetRewardValue);
    }
    public void AttackHitReward()
    {
        AddReward(AttackHitRewardValue);
        //Debug.Log(name + ", AttackHitReward : " + AttackHitRewardValue);
    }
    public void AttackMissReward()
    {
        AddReward(AttackMissRewardValue);
        //Debug.Log(name + ", AttackMissReward : " + AttackMissRewardValue);
    }
    public void AgentHitReward()
    {
        AddReward(AgentHitRewardValue);
        //Debug.Log(name + ", AgentHitReward : " + AgentHitRewardValue);
    }
    public void AgentDieReward()
    {
        SetReward(AgentDieRewardValue);
        //Debug.Log(name + ", AgentDieReward : " + AgentDieRewardValue);
    }

    public void SeeingEnemyReward()
    {
        AddReward(SeeingEnemyRewardValue);
        //Debug.Log(name + ", SeeingEnemyReward : " + SeeingEnemyRewardValue);
    }
}
