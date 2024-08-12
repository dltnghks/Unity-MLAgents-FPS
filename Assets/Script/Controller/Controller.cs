using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Controller : Agent
{
    public GameEnvironment environment;
    public GameAgents myAgent;
}
