using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;


public class LinearRegression : MonoBehaviour
{

    [SerializeField]
    private Transform[] _dots;
    public GameObject PlanDot;
    public int surface;
    
    private void Start()
    {
        
        //-------------------------------------unit test start---------------------------------------
        Matrix<double> inputsTest = DenseMatrix.OfArray(new [,] { {0.0}, {1.0}, {2.0} });
        Matrix<double> targetsTest = DenseMatrix.OfArray(new [,] { {2.0}, {5.0}, {8.0} });
        var modelTest = Train(inputsTest, targetsTest);
        Matrix<double> testValue = DenseMatrix.OfArray(new [,] { {4.0} });
        var resultTest = Predict(modelTest, testValue);
        //unit test result
        var unitTestResult = (resultTest.At(0, 0) == 14.0) ? "Success" : "Failure";
        Debug.Log("Unit test: " + unitTestResult + " !");
        //-------------------------------------------------------------------------------------------
        
        var inputs = Matrix<double>.Build.Dense(_dots.Length, 2, 0.0);
        var targets = Matrix<double>.Build.Dense(_dots.Length, 1, 0.0);

        for (var i = 0; i < _dots.Length; i++)
        {
            //init inputs Matrix
            inputs.SetRow(i, new[] { double.Parse(_dots[i].position.x.ToString()), _dots[i].position.z });
            //init targets Matrix
            targets.SetRow(i, new[] { double.Parse(_dots[i].position.y.ToString()) });
        }
        
        var model = Train(inputs, targets);            
        for (var i = 0; i < surface; i++)
        {
            for (var j = 0; j < surface; j++)
            {
                //Instantiate PlanDot with the predicted Y
                var valueToTest = DenseMatrix.OfArray(new double[,] { {i, j} });
                var result = Predict(model, valueToTest).At(0, 0);
                Instantiate(PlanDot, new Vector3(i, float.Parse(result.ToString()), j), Quaternion.identity);
            }
        }
    }

    private static Matrix<double> Train(Matrix<double> inputs, Matrix<double> targets)
    {
        //add bias
        inputs = inputs.Append(Matrix<double>.Build.Dense(inputs.RowCount, 1, 1.0));
        //pseudo inverse
        return ((inputs.Transpose()*inputs).PseudoInverse()*inputs.Transpose())*targets;
    }

    private static Matrix<double> Predict(Matrix<double> model, Matrix<double> input)
    {
        //add bias
        input = input.Append(Matrix<double>.Build.Dense(input.RowCount, 1, 1.0));
        return input * model;
    }
}

