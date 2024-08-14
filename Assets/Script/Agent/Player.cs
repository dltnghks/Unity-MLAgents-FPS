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
    }

    [Header("Agent")]
    public List<Controller> _controllerList = new List<Controller>();

    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }

    protected void AddReward(ERewardType rewardType)
    {
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
                default:
                    Debug.LogError("정의되지 않은 보상 타입입니다.");
                    break;
            }
        }
    }


}
