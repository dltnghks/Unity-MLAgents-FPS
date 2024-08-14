using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Character
{

    public override bool Init()
    {
        if (!base.Init()) return false;
        return true;
    }
}
