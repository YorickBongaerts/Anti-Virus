using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool IsOccupied = false;
    private MathHelper mathHelper;
    private void Start()
    {
        mathHelper = new MathHelper();
        BuildingBlockScript[] buildingBlocks = GameObject.FindObjectsOfType<BuildingBlockScript>();
            foreach (BuildingBlockScript buildingBlock in buildingBlocks)
            {
                if (mathHelper.IsApproximatelyEqual(buildingBlock.gameObject.transform.position, this.gameObject.transform.position, new Vector3(0.5f, 0, 0.5f)))
                {
                    IsOccupied = true;
                }
            }
    }

    
}
