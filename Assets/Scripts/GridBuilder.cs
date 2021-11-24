using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject GridSquare, GridParent;

    [SerializeField]
    public int GridSizeX, GridSizeY;

    public GameObject[,] Grid;
    private bool ShouldCreate = true;
    private MathHelper mathHelper;

    void Start()
    {
        Grid = new GameObject[GridSizeX, GridSizeY];
        mathHelper = new MathHelper();
        for (int x = 0; x < GridSizeX; x++)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                if (ShouldCreate)
                {
                    GameObject gridSquare = Instantiate(GridSquare, new Vector3(x, 0, y), Quaternion.identity);
                    gridSquare.transform.SetParent(GridParent.transform);
                    Grid[x, y] = gridSquare;
                }
                ShouldCreate = !ShouldCreate;
            }
        }
        GridParent.transform.position -= new Vector3(GridSizeX / 2, 0, GridSizeY / 2);
    }
    public GameObject GetBuildingBlockOfTile(GameObject tile)
    {
        foreach (BuildingBlockScript buildingBlock in GameObject.FindObjectsOfType<BuildingBlockScript>())
        {
            if (mathHelper.IsApproximatelyEqual(buildingBlock.transform.position, tile.transform.position,new Vector3(0.5f,0,0.5f)))
            {
                return buildingBlock.gameObject;
            }
        }
        return null;
    }
}
