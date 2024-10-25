using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;


public class TotalController : Controller
{
    public Vector3 agentPos;
    public override void CollectObservations(VectorSensor sensor)
    {
        // ?êÏù¥?ÑÌä∏???ÑÏû¨ ?ÑÏπòÎ•??ÅÎ? Ï¢åÌëúÎ°?Í≥ÑÏÇ∞
        agentPos = myAgent.transform.position - environment.transform.position;

        // ?ÅÎ? Ï¢åÌëúÎ•??ïÍ∑ú?îÌïòÍ≥?Í¥ÄÏ∏??∞Ïù¥?∞Î°ú Ï∂îÍ? 3, map scaleÎ°??òÎà†Ï§?
        sensor.AddObservation(agentPos / environment.MapSize);

        // Agent rotation 1
        sensor.AddObservation(myAgent.transform.eulerAngles.y / 360);

        var localVelocity = myAgent.transform.InverseTransformDirection(myAgent.rBody.velocity);
        // Agent velocity 3
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.y);
        sensor.AddObservation(localVelocity.z);

        sensor.AddObservation(myAgent.targetDir);
        // Í≥µÍ≤© ?¨Í±∞Î¶¨Î°ú ?¥Î≥¥Í∏?
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

        // ?êÌîÑ
        discreteActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
