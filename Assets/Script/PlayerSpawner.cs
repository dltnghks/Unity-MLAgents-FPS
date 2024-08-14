using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : Spawner
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
