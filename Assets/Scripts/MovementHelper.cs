using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    static public Vector3 MakeDiagonal(Vector3 straightVector)
    {
        Vector3 diagonalVector = Vector3.zero;
        diagonalVector.x = straightVector.x * Mathf.Cos(45 * Mathf.Deg2Rad) - straightVector.z * Mathf.Sin(45 * Mathf.Deg2Rad);
        diagonalVector.z = straightVector.x * Mathf.Sin(45 * Mathf.Deg2Rad) + straightVector.z * Mathf.Cos(45 * Mathf.Deg2Rad);
        return diagonalVector;
    }
}
