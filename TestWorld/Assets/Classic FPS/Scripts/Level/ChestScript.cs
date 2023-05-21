using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public LootDrop drop;
    public int DropChange = 1;

    public Transform FlapTransform;

    bool chestOpen;

    public void SpawnDrop()
    {
        if (!chestOpen)
        {
            drop.SpawnDrop(this.transform, DropChange, 2f);
            if (FlapTransform) FlapTransform.transform.Rotate(new Vector3(0, 0, 90));
            chestOpen = true;
        }
    }
}
