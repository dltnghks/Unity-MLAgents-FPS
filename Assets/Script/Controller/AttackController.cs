using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AttackController : Controller
{
    public override void CollectObservations(VectorSensor sensor)
    {
        // 에이전트의 현재 위치를 상대 좌표로 계산
        var agentPos = myAgent.transform.position - environment.transform.position;

        // 상대 좌표를 정규화하고 관측 데이터로 추가 3, map scale로 나눠줌
        sensor.AddObservation(agentPos / environment.MapSize);

        // Agent rotation 1
        sensor.AddObservation(myAgent.rBody.transform.eulerAngles.y / 360);

        sensor.AddObservation(myAgent.targetDir);
        sensor.AddObservation(myAgent.targetDistance / environment.MapSize);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> act = actions.DiscreteActions;
        base.OnActionReceived(actions);
        myAgent.AttackAction(act[0]);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (myAgent.ShootTime <= 0.0f)
        {
            if (Input.GetMouseButton(0))
            {
                discreteActionsOut[0] = 1;
            }
        }
    }
}
