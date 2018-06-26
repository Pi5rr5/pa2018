// ConsoleApplication.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;
using namespace Eigen;


float* InitWeight(int dimension)
{
	float* weight;
	weight = new float[dimension + 1];
	// init weight between -1.0f & 1.0f
	for (int i = 0; i < dimension + 1; i++)
		weight[i] = (rand() / (RAND_MAX / (2.0f))) - 1.0f;
	return weight;
}

float Classify(float* wei, int weiRows, int weiCols, float* in, int inRows, int inCols, bool addBias)
{
	MatrixXf weight = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weiRows, weiCols);
	MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
	if (addBias)
	{
		inputs.conservativeResize(inputs.rows(), inputs.cols() + 1);
		inputs.col(inputs.cols() - 1).setOnes();
	}
	return (weight * inputs.transpose())(0, 0) > 0.0f ? 1.0f : -1.0f;
}

float* TrainPerceptron(float* wei, int weiRows, int weiCols, float* in, int inRows, int inCols, float* out, int outRows, int outCols, float learningRate, int epoch)
{
	MatrixXf weights = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weiRows, weiCols);
	MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
	MatrixXf outputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(out, outRows, outCols);


	cout << weights << endl << "-----------------------" << endl;
	cout << inputs << endl << "-----------------------" << endl;
	cout << outputs << endl << "-----------------------" << endl;

	for (int i = 0; i < epoch; i++)
	{
		for (int j = 0; j < inRows; j++)
		{
			//W += a(Y^k - g(X^k)X^k
			MatrixXf input = inputs.row(j);
			//add bias
			input.conservativeResize(input.rows(), input.cols() + 1);
			input.col(input.cols() - 1).setOnes();

			//cout << input << endl << "-----------------------" << endl;


			MatrixXf output = outputs.row(j);
			auto guess = Classify(wei, int(weights.rows()), int(weights.cols()), in, int(input.rows()), int(input.cols()), false);
			//cout << output << endl << "-----------------------" << endl;
			float error = (output(0, 0) - guess) * learningRate;
			auto result = input * error;
			weights += result;
		}
	}
	
	float* weightsResult;
	weightsResult = new float[weights.rows()*weights.cols()];
	Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(weightsResult, weights.rows(), weights.cols()) = weights;

	cout << weights << endl << "-----------------------" << endl;
	cout << weightsResult[0] << " | " << weightsResult[1] << " | " << weightsResult[2] << " | " << endl << "-----------------------" << endl;

	return weightsResult;
}


int main()
{
	auto model = InitWeight(2);
	auto inputs = new float[22]
	{	0.0f, 0.0f,
		4.0f, 3.0f,
		3.4f, 1.5f,
		2.5f, 2.3f,
		3.5f, 2.2f,
		0.6f, 0.6f,
		0.3f, 1.0f,
		1.0f, 3.0f,
		2.3f, 3.0f,
		0.4f, 2.5f,
		0.2f, 1.4f	};
	auto outputs = new float[11]{ 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f };
	model = TrainPerceptron(model, 1, 3, inputs, 11, 2, outputs, 11, 1, 0.1f, 1);
	auto value = new float[2]{ 2.0f, 0.75f };
	auto result = Classify(model, 1, 3, value, 1, 2, true);
	cout << result << endl;

	return 0;
}

