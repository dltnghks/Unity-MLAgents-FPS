using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Info")]
    public int TeamID;
    [SerializeField]
    protected int maxHP = 100;
    [SerializeField]
    protected int hp;

    private bool _initialized = false;

    public virtual bool Init()
    {
        if (_initialized) return false;
        gameObject.SetActive(true);
        _initialized = true;
        hp = maxHP;
        return true;
    }

    public int HP
    {
        get { return hp; }
    }

    public virtual int AddHP(int val)
    {
        //Debug.Log("hp : " + hp);
        hp += val;
        //Debug.Log("hp : " + hp);
        if (maxHP < HP)
        {
            hp = maxHP;
            CharacterSetActive(false);
        }
        else if (hp < 0)
        {
            hp = 0;
        }
        return hp;
    }


    public void CharacterSetActive(bool b)
    {
        gameObject.SetActive(b);
        if(b == false)
        {
            _initialized = false;
        }

    }
}
