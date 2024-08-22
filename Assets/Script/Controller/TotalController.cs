using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class TotalController : Controller
{
    public override void CollectObservations(VectorSensor sensor)
    {
        // 에이전트의 현재 위치를 상대 좌표로 계산
        var agentPos = myAgent.transform.position - environment.transform.position;

        // 상대 좌표를 정규화하고 관측 데이터로 추가 3, map scale로 나눠줌
        sensor.AddObservation(agentPos / environment.MapSize);

        // Agent rotation 1
        sensor.AddObservation(myAgent.transform.eulerAngles.y / 360);

        var localVelocity = myAgent.transform.InverseTransformDirection(myAgent.rBody.velocity);
        // Agent velocity 3
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.y);
        sensor.AddObservation(localVelocity.z);

        sensor.AddObservation(myAgent.targetDir);
        // 공격 사거리로 해보기
        //sensor.AddObservation(targetDistance / AttackRange);
        sensor.AddObservation(myAgent.targetDistance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> act = actions.DiscreteActions;
        base.OnActionReceived(actions);
        int[] tmp = { act[0], act[1], act[2], act[3] };
        myAgent.MovementAction(tmp);
        myAgent.AttackAction(act[4]);
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
        if (myAgent.ShootTime <= 0.0f)
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
