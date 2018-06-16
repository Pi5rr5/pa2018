using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class Classification : MonoBehaviour {
	
	[SerializeField]
	private Transform[] _dots;
	public GameObject BlueDot;
	public GameObject RedDot;

	private void Start () {
		//-------------------------------------unit test start---------------------------------------
		var modelTest = InitWeight(2);
		var inputsTest = DenseMatrix.OfArray(new[,] {{1.0, 1.0}, {1.0, 0.0}, {0.0, 1.0}, {0.0, 0.0}});
		var outputsTest = DenseMatrix.OfArray(new[,] {{1.0}, {-1.0}, {-1.0}, {-1.0}});
		modelTest = TrainPerceptron(modelTest, inputsTest, outputsTest, 0.1f, 1);
		var valueTest = DenseMatrix.OfArray(new[,] {{1.0, 0.0} });
		var unitTestResult = Classify(modelTest, valueTest, true) < 0 ? "Success" : "Failure";
		Debug.Log("Unit test: " + unitTestResult + " !");
		//-------------------------------------------------------------------------------------------
        
		var model = InitWeight(2);
		var inputs = Matrix<double>.Build.Dense(_dots.Length, 2, 0.0);
		var targets = Matrix<double>.Build.Dense(_dots.Length, 1, 0.0);
		
		for (var i = 0; i < _dots.Length; i++)
		{
			inputs.SetRow(i, new[] { double.Parse(_dots[i].position.x.ToString()), _dots[i].position.z });
			targets.SetRow(i, new[] { _dots[i].CompareTag("red") ? 1.0 : -1.0 });
		}
		
		model = TrainPerceptron(model, inputs, targets, 0.1f, 1000);
		
		for (var i = 0; i < 20; i++)
		{
		    for (var j = 0; j < 20; j++)
		    {
		        //Instantiate PlanDot
			    var valueToClassify = DenseMatrix.OfArray(new double[,] { {i, j} });
		        var result = Classify(model, valueToClassify, true);
			    Instantiate(result > 0 ? RedDot : BlueDot, new Vector3(i, -1, j), Quaternion.identity);
		    }
		}	
	}
	
	private static Matrix<double> InitWeight(int inputDimension)
	{
		return Matrix<double>.Build.Random(1, inputDimension + 1);
	}
	
	private static int Classify(Matrix<double> weight, Matrix<double> inputs, bool addBias)
	{
		if (addBias)
			inputs = inputs.Append(Matrix<double>.Build.Dense(inputs.RowCount, 1, 1.0));
		return (weight * inputs.Transpose()).At(0, 0) > 0 ? 1 : -1;
	}
	
	private static Matrix<double> TrainPerceptron(Matrix<double> weights, Matrix<double> inputs, Matrix<double> outputs, float learningRate, int epoch)
	{
		for (var i = 0; i < epoch; i++)
		{
			for (var j = 0; j < inputs.RowCount; j++)
			{
				//W += a(Y^k - g(X^k)X^k
				var input = inputs.SubMatrix(j, 1, 0, 2);
				//add bias
				input = input.Append(Matrix<double>.Build.Dense(input.RowCount, 1, 1.0));
				var output = outputs.SubMatrix(j, 1, 0, 1);
				var guess = Classify(weights, input, false);
				var error = (output.At(0, 0) - guess) * learningRate;
				var result = input * error;
				weights += result;
			}
		}
		return weights;
	}
}
