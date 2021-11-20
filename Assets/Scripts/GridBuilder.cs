using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject GridSquare, GridParent;

    [SerializeField]
    private int GridSizeX, GridSizeY;

    public GameObject[,] Grid;
    bool ShouldCreate = true;
    // Start is called before the first frame update
    void Start()
    {
        Grid = new GameObject[GridSizeX, GridSizeY];
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
