using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Vector3 startingPosition;
    Vector3 dragOffset;
    Vector3 mousePos;
    private GridBuilder GridBuilder;
    Vector3 buildingBlockOffset;
    private void Start()
    {
        GridBuilder = GameObject.FindObjectOfType<GridBuilder>();
        buildingBlockOffset = (gameObject.transform.parent.position - gameObject.transform.position);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        startingPosition = gameObject.transform.parent.position;
        mousePos = eventData.position;
        mousePos.z = 8;
        dragOffset = Camera.main.ScreenToWorldPoint(mousePos) - this.gameObject.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        mousePos = eventData.position;
        mousePos.z = 8;
        Vector3 movement = Camera.main.ScreenToWorldPoint(mousePos);
        movement = movement - startingPosition - dragOffset + buildingBlockOffset;
        if (IsApproximatelyEqual(Mathf.Abs(movement.x), Mathf.Abs(movement.z),1f))
        {
            movement.x = Mathf.Clamp(Mathf.Sign(movement.x) * (Mathf.Abs(movement.x) + Mathf.Abs(movement.z)) / 2,-3f - startingPosition.x, 3f - startingPosition.x);
            movement.z = Mathf.Clamp(Mathf.Sign(movement.z) * Mathf.Abs(movement.x), -3f -startingPosition.z, 3f- startingPosition.z);
            movement += startingPosition;
            gameObject.transform.parent.position = movement; //- dragOffset + buildingBlockOffset;
        }
        else
        {
            //reset position of selected piece + forcibly end the dragging, this way you don't get weird offsets on your piece while dragging
            gameObject.transform.parent.position = startingPosition;
            eventData.pointerDrag = null;
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
        GameObject currentSquare = GridBuilder.Grid[0,0];
        //snaps the piece to the grid
        foreach (var gridSquare in GridBuilder.Grid)
        {
            if (gridSquare == null)
            {
                continue;
            }
            if ((gridSquare.transform.position - gameObject.transform.parent.position).magnitude < (currentSquare.transform.position - gameObject.transform.parent.position).magnitude)
            {
                currentSquare = gridSquare;
            }
        }
        gameObject.transform.parent.position = currentSquare.transform.position - buildingBlockOffset;
        //gameObject.transform.parent.position =
            //new Vector3(Snapping.Snap(gameObject.transform.parent.position.x, 1), 0, Snapping.Snap(gameObject.transform.parent.position.z, 1));
        
    }
    bool IsApproximatelyEqual(float a, float b, float treshold)
    {
        return (Mathf.Abs(a - b) < treshold);
    }
    
}
