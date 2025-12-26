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
        // ?�이?�트???�재 ?�치�??��? 좌표�?계산
        agentPos = myAgent.transform.position - environment.transform.position;

        // ?��? 좌표�??�규?�하�?관�??�이?�로 추�? 3, map scale�??�눠�?
        sensor.AddObservation(agentPos / environment.MapSize);

        // Agent rotation 1
        sensor.AddObservation(myAgent.transform.eulerAngles.y / 360);

        var localVelocity = myAgent.transform.InverseTransformDirection(myAgent.rBody.velocity);
        // Agent velocity 3
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.y);
        sensor.AddObservation(localVelocity.z);

        sensor.AddObservation(myAgent.targetDir);
        sensor.AddObservation(myAgent.targetDistance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> act = actions.DiscreteActions;
        base.OnActionReceived(actions);
        int[] tmp = { act[0], act[1], act[2]};
        myAgent.MovementAction(tmp);
        myAgent.AttackAction(act[3]);
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
            discreteActionsOut[2] = 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[2] = 2;
        }
        if (myAgent.ShootTime <= 0.0f)
        {
            if (Input.GetMouseButton(0))
            {
                discreteActionsOut[3] = 1;
            }
        }

        // ?�프
        //discreteActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
