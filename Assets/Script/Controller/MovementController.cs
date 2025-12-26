using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MovementController : Controller
{
    public Vector3 agentPos;
    public override void CollectObservations(VectorSensor sensor)
    {
        // ������Ʈ�� ���� ��ġ�� ��� ��ǥ�� ���
        agentPos = myAgent.transform.position - environment.transform.position;

        // ��� ��ǥ�� ����ȭ�ϰ� ���� �����ͷ� �߰� 3, map scale�� ������
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
        //sensor.AddObservation(myAgent.targetDistance / environment.MapSize);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ActionSegment<int> act = actions.DiscreteActions;
        base.OnActionReceived(actions);
        int[] tmp = { act[0], act[1], act[2]};
        myAgent.MovementAction(tmp);
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

        // ����
        //discreteActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }
}
