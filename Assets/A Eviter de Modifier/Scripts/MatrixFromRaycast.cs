using UnityEngine;
using System.Collections;
using System;

public class MatrixFromRaycast {

    static public int[][] CreateMatrixFromRayCast()
    {
        //size 51 to take last column
        int[][] matrix = new int[51][];

        for (int i = 0; i < 51; i++)
        {
            //size 51 to take last line
            matrix[i] = new int[51];
            for (int j = 0; j < 51; j++)
            {
                var ray = new Ray(new Vector3(i - 25, 5, j - 25), Vector3.down);RaycastHit rch;
                if (Physics.Raycast(ray, out rch, 10f))
                {
                    if (rch.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
                    {
                        matrix[i][j] = LayerMask.NameToLayer("Obstacle");
                    }
                    else if (rch.collider.gameObject.layer == LayerMask.NameToLayer("Goal"))
                        matrix[i][j] = LayerMask.NameToLayer("Goal");
                    else
                        matrix[i][j] = 0;
                }
                else
                {
                    matrix[i][j] = 0;
                }
            }
        }
        return matrix;
    }
}
