using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    protected int maxHP;
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected int attackDelay;

    public int HP
    {
        get { return hp; }
    }

    public void AddHP(int val)
    {
        hp += val;
        if (maxHP < HP)
        {
            hp = maxHP;
        }
        else if (hp < 0)
        {
            hp = 0;

        }
    }
}
