using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;
using CSML;

public class LinearRegression : MonoBehaviour
{

    [SerializeField]
    private Transform[] _dots;
    
    public GameObject PlanDot;
    
    private void Start()
    {
        
        /*-------------------------------------unit test start---------------------------------------*/
        var inputsTest = new Matrix("0.0; 1.0; 2.0");
        var targetsTest = new Matrix("2.0; 5.0; 8.0");
        var modelTest = Train(inputsTest, targetsTest);
        var testValue = new Matrix("4.0");
        var resultTest = Predict(modelTest, testValue);
        //unit test result
        var unitTestResult = (float.Parse(resultTest[1].ToString()) == 14.0) ? "Success" : "Failure";
        Debug.Log("Unit test: " + unitTestResult + " !");
        /*-------------------------------------------------------------------------------------------*/
        
        var inputs = new Matrix();
        var targets = new Matrix();
        
        for (var i = 0; i < _dots.Length; i++)
        {
            /*init inputs Matrix*/
            var tmp = _dots[i].position.x + "," + _dots[i].position.z;
            inputs.InsertRow(new Matrix(tmp), i + 1);
            /*init targets Matrix*/
            targets.InsertRow(new Matrix(_dots[i].position.y.ToString()), i + 1);
        }

        var model = Train(inputs, targets);            
        for (var i = 0; i < 30; i++)
        {
            for (var j = 0; j < 30; j++)
            {
                /*Instantiate PlanDot with the predicted Y*/
                var valueToTest = new Matrix(i + "," + j);
                var result = Predict(model, valueToTest);
                Instantiate(PlanDot, new Vector3(i, float.Parse(result[1].ToString()), j), Quaternion.identity);
            }
        }
    }

    private static Matrix Train(Matrix inputs, Matrix targets)
    {
        //add bias
        inputs.InsertColumn(Matrix.Ones(inputs.RowCount, 1), inputs.ColumnCount + 1);
        //pseudo inverse
        return ((inputs.Transpose()*inputs).Inverse()*inputs.Transpose())*targets;
    }

    private static Matrix Predict(Matrix model, Matrix input)
    {
        //add bias
        input.InsertColumn(Matrix.Ones(input.RowCount, 1), input.ColumnCount + 1);
        return input * model;
    }
}

