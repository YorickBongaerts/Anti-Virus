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

    // Update is called once per frame
    void Update()
    {
       
    }
}
