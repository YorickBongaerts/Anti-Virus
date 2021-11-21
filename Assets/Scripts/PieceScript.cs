using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceScript : MonoBehaviour, IDragHandler,IBeginDragHandler, IEndDragHandler
{
    Vector3 startingPosition;
    public void OnBeginDrag(PointerEventData eventData)
    {
        startingPosition = gameObject.transform.parent.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //FirstTry();
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 8;
        Vector3 movement = Camera.main.ScreenToWorldPoint(mousePos);
        movement = movement - startingPosition;
        if (IsApproximatelyEqual(Mathf.Abs(movement.x), Mathf.Abs(movement.z),1f))
        {
            //probably snap in here instead of at the end (or maybe snap at both points in time)
            movement.x = Mathf.Clamp(Mathf.Sign(movement.x) * (Mathf.Abs(movement.x) + Mathf.Abs(movement.z)) / 2,-3.5f - startingPosition.x, 3.5f - startingPosition.x);
            movement.z = Mathf.Clamp(Mathf.Sign(movement.z) * Mathf.Abs(movement.x), -3.5f -startingPosition.z, 3.5f- startingPosition.z);
            movement += startingPosition;
            gameObject.transform.parent.position = movement;
        }
        else
        {
            gameObject.transform.parent.position = startingPosition;
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
        //snaps the piece to the grid
        gameObject.transform.parent.position =
            new Vector3(Snapping.Snap(gameObject.transform.parent.position.x, 1), 0, Snapping.Snap(gameObject.transform.parent.position.z, 1));
        
    }

    bool IsApproximatelyEqual(float a, float b, float treshold)
    {
        return (Mathf.Abs(a - b) < treshold);
    }
    // Update is called once per frame
    void Update()
    {
       
    }
}
