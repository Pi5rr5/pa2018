// ConsoleApplication.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

using namespace std;
using namespace Eigen;


float* InitWeight(int dimension)
{
	float* weight;
	weight = new float[dimension + 1];
	srand((int)time(0));
	for (int i = 0; i < dimension + 1; i++)
		weight[i] = 1;
		//weight[i] = (rand() / (RAND_MAX / (2.0f))) - 1.0f;
	return weight;
}

float Classify(float* wei, int weiRows, int weiCols, float* in, int inRows, int inCols, bool addBias)
{
	MatrixXf weight = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weiRows, weiCols);
	MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);

	//cout << endl << "----weight-------" << endl << weight << endl << "---------------" << endl;
	//cout << endl << "----inputs-------" << endl << inputs << endl << "---------------" << endl;

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

	for (int i = 0; i < epoch; i++)
	{
		for (int j = 0; j < inRows; j++)
		{
			//cout << endl << "----weights-------" << endl << weights << endl << "---------------" << endl;

			//W += a(Y^k - g(X^k)X^k
			MatrixXf input = inputs.row(j);
			//add bias
			input.conservativeResize(input.rows(), input.cols() + 1);
			input.col(input.cols() - 1).setOnes();
			//cout << endl << "----input-------" << endl << input << endl << "---------------" << endl;
			float* mInput;
			mInput = new float[input.rows() * input.cols()];
			Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(mInput, input.rows(), input.cols()) = input;

			MatrixXf output = outputs.row(j);
			//cout << output << endl;
			//cout << "weight avant classif" << endl;
			//cout << wei[0] << " | " << wei[1] << " | " << wei[2] << " | " << endl;
			float guess = Classify(wei, int(weights.rows()), int(weights.cols()), mInput, int(input.rows()), int(input.cols()), false);

			//cout << endl << "----guess-------" << endl << guess << endl << "---------------" << endl;

			float error = (output(0, 0) - guess) * learningRate;
			//cout << guess << endl;
			MatrixXf result = input * error;
			weights += result;
			Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weights.rows(), weights.cols()) = weights;
		}
	}
	
	float* weightsResult;
	weightsResult = new float[weights.rows()*weights.cols()];
	Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(weightsResult, weights.rows(), weights.cols()) = weights;
	return weightsResult;
}


int main()
{
	auto model = InitWeight(2);
	cout << model[0] << " | " << model[1] << " | " << model[2] << " | " << endl;

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

