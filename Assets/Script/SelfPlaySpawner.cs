using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfPlaySpawner : Spawner
{
    public override void Clear()
    {
        foreach (var obj in spawnObjectList)
        {
            obj.GetComponent<Character>().CharacterSetActive(false);
        }
        //spawnObjectList.Clear();
    }
}
