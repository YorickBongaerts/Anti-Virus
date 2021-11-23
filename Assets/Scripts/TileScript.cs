using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool IsOccupied = false;
    private void Start()
    {
        BuildingBlockScript[] buildingBlocks = GameObject.FindObjectsOfType<BuildingBlockScript>();
            foreach (BuildingBlockScript buildingBlock in buildingBlocks)
            {
                if (IsApproximatelyEqual(buildingBlock.gameObject.transform.position, this.gameObject.transform.position, new Vector3(0.5f, 0, 0.5f)))
                {
                    IsOccupied = true;
                }
            }
    }
    bool IsApproximatelyEqual(Vector3 a, Vector3 b, Vector3 treshold)
    {
        return (Mathf.Abs(a.x - b.x) < treshold.x && Mathf.Abs(a.z - b.z) < treshold.z);
    }
}
