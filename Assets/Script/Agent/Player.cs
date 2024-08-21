using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Character
{

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
            //Debug.Log(name + " : " + rewardType.ToString());
        foreach (var controller in _controllerList)
        {
            switch (rewardType)
            {
                case ERewardType.KillTarget:
                    controller.KillTargetReward();
                    //bKill = true;
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
                    //bDeath = true;
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
