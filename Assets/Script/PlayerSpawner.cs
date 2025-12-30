using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : Spawner
{
    public override void Clear()
    {
        foreach (var obj in spawnObjectList)
        {
            if (obj != null)
            {
                var character = obj.GetComponent<Character>();
                if (character != null)
                {
                    character.CharacterSetActive(false);
                }
            }
        }
        //spawnObjectList.Clear();
    }
}
