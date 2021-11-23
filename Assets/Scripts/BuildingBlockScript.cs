using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingBlockScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector3 startingPosition;
    Vector3 dragOffset;
    Vector3 mousePos;
    Vector3 movement;
    private GridBuilder GridBuilder;
    Vector3 buildingBlockOffset;
    public bool IsBlocker = false;
    private void Start()
    {
        GridBuilder = GameObject.FindObjectOfType<GridBuilder>();
        buildingBlockOffset = (gameObject.transform.parent.position - gameObject.transform.position);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsBlocker)
        {
            UnoccupyTiles();
            startingPosition = gameObject.transform.parent.position;
            mousePos = eventData.position;
            mousePos.z = 8;
            dragOffset = Camera.main.ScreenToWorldPoint(mousePos) - this.gameObject.transform.position;
        }
    }

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
                if (IsApproximatelyEqual(child.transform.position, tile.transform.position, new Vector3(0.5f, 0, 0.5f)))
                {
                    tile.GetComponent<TileScript>().IsOccupied = false;
                    Debug.Log("tile became unoccupied");
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsBlocker)
        {
            mousePos = eventData.position;
            mousePos.z = 8;
            movement = Camera.main.ScreenToWorldPoint(mousePos);
            movement = movement - startingPosition - dragOffset + buildingBlockOffset;
            if (IsApproximatelyEqual(Mathf.Abs(movement.x), Mathf.Abs(movement.z), 1f))
            {
                movement.x = Mathf.Sign(movement.x) * (Mathf.Abs(movement.x) + Mathf.Abs(movement.z)) / 2;
                movement.z = Mathf.Sign(movement.z) * Mathf.Abs(movement.x);
                movement += startingPosition;
                gameObject.transform.parent.position = movement;
                foreach (Transform child in gameObject.transform.parent.GetComponentsInChildren<Transform>())
                {
                    if (child.transform.position.x > 3.5 || child.transform.position.x < -3.5 || child.transform.position.z > 3.5 || child.transform.position.z < -3.5)
                    {
                        gameObject.transform.parent.position = startingPosition;
                        eventData.pointerDrag = null;
                        OccupyTiles();
                    }
                }
            }
            else
            {
                //reset position of selected piece + forcibly end the dragging, this way you don't get weird offsets on your piece while dragging
                gameObject.transform.parent.position = startingPosition;
                eventData.pointerDrag = null;
                OccupyTiles();
            }
            PieceInteractionBlock(eventData);
        }
    }

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
                if (tile.GetComponent<TileScript>().IsOccupied && IsApproximatelyEqual(child.transform.position,tile.transform.position,new Vector3(0.5f,0,0.5f)))
                {
                    Debug.Log("piece is blocked");
                    gameObject.transform.parent.position = startingPosition;
                    eventData.pointerDrag = null;
                    OccupyTiles();
                }
            }
        }
    }

    /// <summary>
    /// kind of works, but not really
    /// </summary>
    private void FirstTry()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 8;
        Vector3 rawMovement = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 diagonalMovement = MovementHelper.MakeDiagonal(rawMovement);
        if (Mathf.Abs(diagonalMovement.x - startingPosition.x) > Mathf.Abs(diagonalMovement.z - startingPosition.z))
        {
            rawMovement.z = startingPosition.z;
        }
        else
        {
            rawMovement.x = startingPosition.x;
        }
        diagonalMovement = MovementHelper.MakeDiagonal(rawMovement);
        gameObject.transform.parent.position = diagonalMovement;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsBlocker)
        {
            GameObject currentSquare = GridBuilder.Grid[0, 0];
            //snaps the piece to the grid
            foreach (var gridSquare in GridBuilder.Grid)
            {
                if (gridSquare == null)
                {
                    continue;
                }
                if ((gridSquare.transform.position - gameObject.transform.position).magnitude < (currentSquare.transform.position - gameObject.transform.position).magnitude)
                {
                    currentSquare = gridSquare;
                }
            }
            gameObject.transform.parent.position = currentSquare.transform.position + buildingBlockOffset;
            OccupyTiles();
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
                if (IsApproximatelyEqual(child.transform.position, tile.transform.position, new Vector3(0.5f, 0, 0.5f)))
                {
                    tile.GetComponent<TileScript>().IsOccupied = true;
                    Debug.Log("tile became occupied");
                }
            }
        }
    }

    bool IsApproximatelyEqual(float a, float b, float treshold)
    {
        return (Mathf.Abs(a - b) < treshold);
    }
    bool IsApproximatelyEqual(Vector3 a, Vector3 b, Vector3 treshold)
    {
        return (Mathf.Abs(a.x - b.x) < treshold.x && Mathf.Abs(a.z - b.z) < treshold.z);
    }

}
