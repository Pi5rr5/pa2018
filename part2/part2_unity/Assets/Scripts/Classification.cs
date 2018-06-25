using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Classification : MonoBehaviour {
	
	[SerializeField]
	private Transform[] _dots;
	public GameObject BlueDot;
	public GameObject RedDot;
	public bool NonLinearDeep;
	public float LearningRate;
	public int Epochs;
	
	[DllImport("Unity_dll")]
	private static extern IntPtr InitWeight(int dimension);
	
	[DllImport("Unity_dll")]
	private static extern float Classify(IntPtr weights, int weightsRows, int weightsCols, float[] inputs, int inputsRows, int inputsCols, bool addBias);
	
	[DllImport("Unity_dll")]
	private static extern IntPtr TrainPerceptron(IntPtr weights, int weightsRows, int weightsCols, float[] inputs, int inputsRows, int inputsCols, float[] outputs, int outputsRows, int outputsCols, float learningRate, int epoch);


	private void Start()
	{
		var model = NonLinearDeep ? InitWeight(dimension: 1) : InitWeight(dimension: 2);
		var dotsInputs = new List<float>();
		var dotsTargets = new List<float>();
		
		foreach (var dot in _dots)
		{
			if (NonLinearDeep)
			{
				dotsInputs.Add(item: dot.position.x * dot.position.z);
			}
			else
			{
				dotsInputs.Add(item: dot.position.x);
				dotsInputs.Add(item: dot.position.z);
			}

			dotsTargets.Add(item: dot.CompareTag(tag: "red") ? 1.0f : -1.0f);
		}
		
		if(NonLinearDeep)
			model = TrainPerceptron(weights: model, weightsRows: 1, weightsCols: 3, inputs: dotsInputs.ToArray(), inputsRows: dotsInputs.Count, inputsCols: 1, outputs: dotsTargets.ToArray(), outputsRows: dotsTargets.Count, outputsCols: 1, learningRate: LearningRate, epoch: Epochs);
		else
			model = TrainPerceptron(weights: model, weightsRows: 1, weightsCols: 3, inputs: dotsInputs.ToArray(), inputsRows: dotsInputs.Count / 2, inputsCols: 2, outputs: dotsTargets.ToArray(), outputsRows: dotsTargets.Count, outputsCols: 1, learningRate: LearningRate, epoch: Epochs);
		for (var i = -10f; i <= 10f; i += 1f)
		{
			for (var j = -10f; j <= 10f; j += 1f)
			{
				//Instantiate PlanDot
				var valueToClassify = new float[] {i, j};
				var result = Classify(weights: model, weightsRows: 1, weightsCols: 3, inputs: valueToClassify, inputsRows: 1, inputsCols: 2, addBias: true);
				//Debug.Log(result);
				Instantiate(original: result > 0 ? RedDot : BlueDot, position: new Vector3(x: i, y: -1, z: j), rotation: Quaternion.identity);
			}
		}
	}


	/*
	private void Start () {
		//-------------------------------------unit test start---------------------------------------
		var modelTest = InitWeight(2);
		var inputsTest = DenseMatrix.OfArray(new[,] {{1.0, 1.0}, {1.0, 0.0}, {0.0, 1.0}, {0.0, 0.0}});
		var outputsTest = DenseMatrix.OfArray(new[,] {{1.0}, {-1.0}, {-1.0}, {-1.0}});
		modelTest = TrainPerceptron(modelTest, inputsTest, outputsTest, 0.1f, 1);
		var valueTest = DenseMatrix.OfArray(new[,] {{1.0, 	0.0} });
		var unitTestResult = Classify(modelTest, valueTest, true) < 0 ? "Success" : "Failure";
		Debug.Log("Unit test: " + unitTestResult + " !");
		//-------------------------------------------------------------------------------------------
        
		var model = NonLinearDeep ? InitWeight(3) : InitWeight(2);
		var inputs = Matrix<double>.Build.Dense(_dots.Length, NonLinearDeep ? 3 : 2, 0.0);
		var targets = Matrix<double>.Build.Dense(_dots.Length, 1, 0.0);
		
		for (var i = 0; i < _dots.Length; i++)
		{
			if (NonLinearDeep)
				inputs.SetRow(i, new[] { double.Parse(_dots[i].position.x.ToString()), _dots[i].position.z, _dots[i].position.x * _dots[i].position.z });
			else
				inputs.SetRow(i, new[] { double.Parse(_dots[i].position.x.ToString()), _dots[i].position.z });
			targets.SetRow(i, new[] { _dots[i].CompareTag("red") ? 1.0 : -1 });
		}
		
		model = TrainPerceptron(model, inputs, targets, 0.01f, 1000);
		
		for (var i = -10; i <= 10; i++)
		{
		    for (var j = -10; j <= 10; j++)
		    {
		        //Instantiate PlanDot
			    var valueToClassify = NonLinearDeep ? DenseMatrix.OfArray(new double[,] { {i, j, i*j} }) : DenseMatrix.OfArray(new double[,] { {i, j} });
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
				var input = inputs.SubMatrix(j, 1, 0, inputs.ColumnCount);
				//add bias
				input = input.Append(Matrix<double>.Build.Dense(input.RowCount, 1, 1.0));
				var output = outputs.SubMatrix(j, 1, 0, 1);
				Debug.Log(weights.RowCount);
				Debug.Log(weights.ColumnCount);
				Debug.Log(input.RowCount);
				Debug.Log(input.ColumnCount);
				var guess = Classify(weights, input, false);
				var error = (output.At(0, 0) - guess) * learningRate;
				var result = input * error;
				weights += result;
			}
		}
		return weights;
	}
	*/
}
