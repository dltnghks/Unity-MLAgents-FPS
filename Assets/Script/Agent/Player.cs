using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public struct FSaveData
    {
        public int KillCount;
        public int DeathCount;
        public int HitCount;      // 내가 맞은 횟수 (Times I was hit)
        public int AttackCount;   // 내 공격이 성공(명중)한 횟수 (Successful attacks I landed)
        public int MissCount;     // 내 공격이 빗나간 횟수 (Attacks I missed)

        public void ResetCount()
        {
            KillCount = 0;
            DeathCount = 0;
            HitCount = 0;
            AttackCount = 0;
            MissCount = 0;
        }
    }

    public struct FGameData
    {
        public int HitCount;
        public int AttackCount;
        public int MissCount;
    }

    protected enum ERewardType
    {
        KillTarget,
        AttackHit,  // Attack was successful (it hit an enemy)
        AttackMiss, // Attack missed
        AgentHit,   // This agent was hit by an enemy
        AgentDie,
        Tick,
        SeeingEnemy,
    }

    [Header("Agent")]
    public List<Controller> _controllerList = new List<Controller>();

    public FSaveData _saveData;
    public FGameData _gameData;

    public override bool Init()
    {
        if (!base.Init()) return false;
        bDeath = false;
        bKill = false;
        return true;
    }

    private bool bDeath = false;
    private bool bKill = false;
    protected void AddReward(ERewardType rewardType)
    {
        if (bDeath || bKill)
        {
            return;
        }
        //Debug.Log(rewardType);
        switch (rewardType)
        {
            case ERewardType.KillTarget:
                _saveData.KillCount++;
                break;
            case ERewardType.AttackHit:
                _saveData.AttackCount++; // 내 공격이 성공했으므로 AttackCount 증가
                break;
            case ERewardType.AttackMiss:
                _saveData.MissCount++;
                break;
            case ERewardType.AgentHit:
                _saveData.HitCount++; // 내가 피격 당했으므로 HitCount 증가
                break;
            case ERewardType.AgentDie:
               _saveData.DeathCount++;
                break;
            case ERewardType.Tick:
                break;
            case ERewardType.SeeingEnemy:
                break;
            default:
                Debug.LogError("Undefined reward type.");
                break;
        }
        
        foreach(var controller in _controllerList)
        {
            switch (rewardType)
            {
                case ERewardType.KillTarget:
                    controller.KillTargetReward();
                    break;
                case ERewardType.AttackHit:
                    controller.AttackHitReward();
                    break;
                case ERewardType.AttackMiss:
                    controller.AttackMissReward();
                    break;
                case ERewardType.AgentHit:
                    controller.AgentHitReward();
                    break;
                case ERewardType.AgentDie:
                    controller.AgentDieReward();
                    break;
                case ERewardType.Tick:
                    controller.TickReward();
                    break;
                case ERewardType.SeeingEnemy:
                    controller.SeeingEnemyReward();
                    break;
                default:
                    Debug.LogError("Undefined reward type for controller.");
                    break;
            }
        }
    }
}
