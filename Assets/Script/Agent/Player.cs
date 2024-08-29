using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Character
{
    public struct FSaveData
    {
        public int KillCount;
        public int DeathCount;
        public int HitCount;
        public int AttackCount;
        public int MissCount;
    }
    
    
    protected enum ERewardType
    {
        KillTarget,
        AttackHit,
        AttackMiss,
        AgentHit,
        AgentDie,
        Tick,
        SeeingEnemy,
    }

    [Header("Agent")]
    public List<Controller> _controllerList = new List<Controller>();

    public FSaveData _saveData;
    
    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }

    protected void AddReward(ERewardType rewardType)
    {
        if (GameManager._instance.IsTest)
        {
            switch (rewardType)
            {
                case ERewardType.KillTarget:
                    _saveData.KillCount++;
                    break;
                case ERewardType.AttackHit:
                    _saveData.AttackCount++;
                    break;
                case ERewardType.AttackMiss:
                    _saveData.MissCount++;
                    break;
                case ERewardType.AgentHit:
                    _saveData.HitCount++;
                    break;
                case ERewardType.AgentDie:
                    _saveData.DeathCount++;
                    break;
                case ERewardType.Tick:
                    break;
                case ERewardType.SeeingEnemy:
                    break;
                default:
                    Debug.LogError("���ǵ��� ���� ���� Ÿ���Դϴ�.");
                    break;
            }
        }



        //Debug.Log(name + " : " + rewardType.ToString());
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
                    Debug.LogError("���ǵ��� ���� ���� Ÿ���Դϴ�.");
                    break;
            }
        }
    }
    
}
