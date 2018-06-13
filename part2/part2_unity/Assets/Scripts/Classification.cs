using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CSML;

public class Classification : MonoBehaviour {
	
	[SerializeField]
	private Transform[] _dots;
	public GameObject BlueDot;
	public GameObject RedDot;

	private void Start () {
		/*-------------------------------------unit test start---------------------------------------*/
		var modelTest = InitWeight(2);
		modelTest = TrainPerceptron(modelTest, new Matrix("1.0, 1.0; 1.0, 0.0; 0.0, 1.0; 0.0, 0.0"), new Matrix("1; -1; -1; -1"), 0.1f, 1);
		Debug.Log(Classify(modelTest, new Matrix("1.0, 0.0"), true));
		/*-------------------------------------------------------------------------------------------*/
        
		var model = InitWeight(2);
		var inputs = new Matrix();
		var targets = new Matrix();
		for (var i = 0; i < _dots.Length; i++)
		{
			inputs.InsertRow(new Matrix(_dots[i].position.x + "," + _dots[i].position.z), i + 1);
			targets.InsertRow(new Matrix(_dots[i].CompareTag("red") ? "1" : "-1"), i + 1);
		}
		
		model = TrainPerceptron(model, inputs, targets, 0.1f, 1000);
		
		for (var i = 0; i < 20; i++)
		{
		    for (var j = 0; j < 20; j++)
		    {
		        //Instantiate PlanDot
		        var valueToTest = new Matrix(i + "," + j);
		        var result = Classify(model, valueToTest, true);
			    Instantiate(result > 0 ? RedDot : BlueDot, new Vector3(i, -1, j), Quaternion.identity);
		    }
		}		
	}
	
	private static Matrix InitWeight(int inputDimension)
	{
		return Matrix.Random(1, inputDimension + 1);;
	}

	private static int Classify(Matrix weight, Matrix inputs, bool addBias)
	{
		if (addBias)
			inputs.InsertColumn(Matrix.Ones(inputs.RowCount, 1), inputs.ColumnCount + 1);
		return float.Parse((weight * inputs.Transpose())[1].ToString()) > 0 ? 1 : -1;
	}

	private static Matrix TrainPerceptron(Matrix weights, Matrix inputs, Matrix outputs, float learningRate, int epoch)
	{
		for (var i = 0; i < epoch; i++)
		{
			for (var j = 1; j <= inputs.RowCount; j++)
			{
				//W += a(Y^k - g(X^k)X^k
				var input = inputs.Row(j);
				//add bias
				input.InsertRow(Matrix.Ones(1, 1), input.RowCount + 1);
				var output = outputs.Row(j);
				var guess = Classify(weights, input.Transpose(), false);
				var error = (int.Parse(output[1].ToString()) - guess) * learningRate;
				var result = input * error;
				weights += result.Transpose();
			}
		}
		return weights;
	}
}
