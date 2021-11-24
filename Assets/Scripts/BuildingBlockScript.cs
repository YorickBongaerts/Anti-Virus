using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingBlockScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region Variables
    private Vector3 pieceStartingPosition;
    private Vector3 mouseToCenterOfBuildingBlockDragOffset;
    private Vector3 mousePosition;
    private Vector3 dragToPosition;
    private Vector3 buildingBlockToPieceCenterOffset;
    public bool IsBlocker = false;
    private GridBuilder GridBuilder;
    private MathHelper mathHelper;
    GameObject NewlyAttachedPiece;
    #endregion Variables

    private void Start()
    {
        GridBuilder = GameObject.FindObjectOfType<GridBuilder>();
        mathHelper = new MathHelper();
        buildingBlockToPieceCenterOffset = (gameObject.transform.parent.position - gameObject.transform.position);
    }
    
    #region DragHandling
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsBlocker)
        {
            UnoccupyTiles();
            pieceStartingPosition = gameObject.transform.parent.position;
            mousePosition = eventData.position;
            mousePosition.z = 8;
            mouseToCenterOfBuildingBlockDragOffset = Camera.main.ScreenToWorldPoint(mousePosition) - this.gameObject.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsBlocker)
        {
            mousePosition = eventData.position;
            mousePosition.z = 8;
            dragToPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            dragToPosition = dragToPosition - pieceStartingPosition - mouseToCenterOfBuildingBlockDragOffset + buildingBlockToPieceCenterOffset;
            if (mathHelper.IsApproximatelyEqual(Mathf.Abs(dragToPosition.x), Mathf.Abs(dragToPosition.z), 1f))
            {
                //make sure the piece slides perfectly on diagonals by converting the components to have the same absolute value
                dragToPosition.x = Mathf.Sign(dragToPosition.x) * (Mathf.Abs(dragToPosition.x) + Mathf.Abs(dragToPosition.z)) / 2;
                dragToPosition.z = Mathf.Sign(dragToPosition.z) * Mathf.Abs(dragToPosition.x);
                
                dragToPosition += pieceStartingPosition;
                gameObject.transform.parent.position = dragToPosition;
                foreach (Transform child in gameObject.transform.parent.GetComponentsInChildren<Transform>())
                {
                    if (child.transform.position.x > GridBuilder.GridSizeX ||
                        child.transform.position.x < -GridBuilder.GridSizeX ||
                        child.transform.position.z > GridBuilder.GridSizeY ||
                        child.transform.position.z < -GridBuilder.GridSizeY)
                    {
                        ForceEndDrag(eventData);
                        //NewlyAttachedPiece?.transform.SetParent(null);
                    }
                }
            }
            else
            {
                //reset position of selected piece + forcibly end the dragging, this way you don't get weird offsets on your piece while dragging
                ForceEndDrag(eventData);
                //NewlyAttachedPiece?.transform.SetParent(null);
            }
            PieceInteractionBlock(eventData);
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsBlocker)
        {
            SnapPieceToGrid();
            NewlyAttachedPiece?.transform.SetParent(null);
            OccupyTiles();
        }
    }

    /// <summary>
    /// Cycles through all tiles of the grid to see which one is closest to this building block and then snaps the block to that tile
    /// </summary>
    private void SnapPieceToGrid()
    {
        GameObject CurrentClosestTile = GridBuilder.Grid[0, 0];
        float currentClosestTileDistance = 100;
        //snaps the piece to the grid
        foreach (var gridSquare in GridBuilder.Grid)
        {
            if (gridSquare == null)
            {
                continue;
            }
            float currentTileDistance = (gridSquare.transform.position - gameObject.transform.position).magnitude;
            currentClosestTileDistance = (CurrentClosestTile.transform.position - gameObject.transform.position).magnitude;
            if (currentTileDistance < currentClosestTileDistance)
            {
                CurrentClosestTile = gridSquare;
            }
        }
        gameObject.transform.parent.position = CurrentClosestTile.transform.position + buildingBlockToPieceCenterOffset;
    }

    private void ForceEndDrag(PointerEventData eventData)
    {
        gameObject.transform.parent.position = pieceStartingPosition;
        eventData.pointerDrag = null;
        NewlyAttachedPiece?.transform.SetParent(null);
        GameObject[] attachedBuildingBlocks = NewlyAttachedPiece?.GetComponentsInChildren<GameObject>();
        foreach (GameObject attachedBuildingBlock in attachedBuildingBlocks)
        {
            attachedBuildingBlock.GetComponent<BuildingBlockScript>().SnapPieceToGrid();
            attachedBuildingBlock.GetComponent<BuildingBlockScript>().OccupyTiles();
        }
        OccupyTiles();
    }

    #endregion DragHandling

    private void PieceInteractionBlock(PointerEventData eventData)
    {
        foreach (Transform child in gameObject.transform.parent.GetComponentsInChildren<Transform>())
        {
            foreach (GameObject tile in GridBuilder.Grid)
            {
                if (tile == null)
                {
                    continue;
                }
                if (tile.GetComponent<TileScript>().IsOccupied && mathHelper.IsApproximatelyEqual(child.transform.position, tile.transform.position, new Vector3(0.9f, 0, 0.9f)))
                {
                    Debug.Log("piece is blocked by another piece");
                    
                    NewlyAttachedPiece = GridBuilder.GetBuildingBlockOfTile(tile).transform.parent.gameObject;
                    if (NewlyAttachedPiece.GetComponentInChildren<BuildingBlockScript>().IsBlocker)
                    {
                        ForceEndDrag(eventData);
                        return;
                    }
                    NewlyAttachedPiece.transform.SetParent(this.gameObject.transform.parent);
                }
            }
        }
    }

    #region TileOccupation
    private void UnoccupyTiles()
    {
        foreach (Transform child in gameObject.transform.parent.GetComponentsInChildren<Transform>())
        {
            foreach (GameObject tile in GridBuilder.Grid)
            {
                if (tile == null)
                {
                    continue;
                }
                if (mathHelper.IsApproximatelyEqual(child.transform.position, tile.transform.position, new Vector3(0.5f, 0, 0.5f)))
                {
                    tile.GetComponent<TileScript>().IsOccupied = false;
                    Debug.Log("tile at" + tile.transform.position + " became unoccupied");
                }
            }
        }
    }
    private void OccupyTiles()
    {
        foreach (Transform child in gameObject.transform.parent.GetComponentsInChildren<Transform>())
        {
            foreach (GameObject tile in GridBuilder.Grid)
            {
                if (tile == null)
                {
                    continue;
                }
                if (mathHelper.IsApproximatelyEqual(child.transform.position, tile.transform.position, new Vector3(0.5f, 0, 0.5f)))
                {
                    tile.GetComponent<TileScript>().IsOccupied = true;
                    Debug.Log("tile at" + tile.transform.position + " became occupied");
                }
            }
        }
    }
    #endregion TileOccupation

}
