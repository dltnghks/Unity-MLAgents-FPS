using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    protected int maxHP = 100;
    [SerializeField]
    protected int hp;

    public virtual void Init()
    {
        hp = maxHP;
    }

    public int HP
    {
        get { return hp; }
    }

    public virtual int AddHP(int val)
    {
        Debug.Log("hp : " + hp);
        hp += val;
        Debug.Log("hp : " + hp);
        if (maxHP < HP)
        {
            hp = maxHP;
        }
        else if (hp < 0)
        {
            hp = 0;
        }
        return hp;
    }
}
