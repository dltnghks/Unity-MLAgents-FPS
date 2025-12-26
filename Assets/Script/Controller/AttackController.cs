using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AttackController : Controller
{
    public Vector3 agentPos;
    public override void CollectObservations(VectorSensor sensor)
    {
        // ������Ʈ�� ���� ��ġ�� ��� ��ǥ�� ���
        agentPos = myAgent.transform.position - environment.transform.position;

        // ��� ��ǥ�� ����ȭ�ϰ� ���� �����ͷ� �߰� 3, map scale�� ������
        sensor.AddObservation(agentPos / environment.MapSize);

        // Agent rotation 1
        sensor.AddObservation(myAgent.rBody.transform.eulerAngles.y / 360);

        sensor.AddObservation(myAgent.targetDir);
        sensor.AddObservation(myAgent.targetDistance);
        //sensor.AddObservation(myAgent.targetDistance / environment.MapSize);
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
