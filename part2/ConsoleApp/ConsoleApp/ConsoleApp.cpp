#include "stdafx.h"

using namespace std;
using namespace Eigen;

double* train(double*, int, int, double*, int, int);
double predict(double*, int, int, double*, int, int);

float* InitWeight(int);
float Classify(float*, int, int, float*, int, int, bool);
float* TrainPerceptron(float*, int, int, float*, int, int, float*, int, int, float, int);


int main()
{
	/*
	double[, ] inputs = { { 0.0 },{ 1.0 },{ 2.0 } };
	double[, ] outputs = { { 2.0 },{ 5.0 },{ 8.0 } };
	var model = train(inputs, inputs.GetLength(0), inputs.GetLength(1), outputs, outputs.GetLength(0), outputs.GetLength(1));
	double[,] test = {{4.0}};
	var result = predict(model, model.GetLength(0), model.GetLength(1), test, test.GetLength(0), test.GetLength(1));	*/

	/*
	double* inputs = new double[3]{ 0.0, 1.0, 2.0 };
	double* outputs = new double[3]{ 2.0, 5.0, 8.0 };
	auto model = train(inputs, 3, 1, outputs, 3, 1);
	auto test = new double[1]{ 4.0 };
	auto result = predict(model, 2, 3, test, 1, 1);
	//cout << result << endl;

	for (int i = 0; i < 100; i++)
		cout << (rand() / (RAND_MAX / (2.0))) -1 << endl;

	//cin >> result;
	*/

	/*
	var modelTest = InitWeight(2);
	var inputsTest = DenseMatrix.OfArray(new[, ]{ { 1.0, 1.0 },{ 1.0, 0.0 },{ 0.0, 1.0 },{ 0.0, 0.0 } });
	var outputsTest = DenseMatrix.OfArray(new[, ]{ { 1.0 },{ -1.0 },{ -1.0 },{ -1.0 } });
	modelTest = TrainPerceptron(modelTest, inputsTest, outputsTest, 0.1f, 1);
	var valueTest = DenseMatrix.OfArray(new[, ]{ { 1.0, 	0.0 } });
	var unitTestResult = Classify(modelTest, valueTest, true) < 0 ? "Success" : "Failure";
	Debug.Log("Unit test: " + unitTestResult + " !");
	*/

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


float* InitWeight(int dimension)
{
	float* weight;
	weight = new float[dimension + 1];
	// init weight between -1 & 1
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
	//cout << "--input--" << endl << inputs << endl << "---------------" << endl;
	//cout << "--result--" << endl << (weight * inputs.transpose()) << endl << "---------------" << endl;
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
			//W += a(Y^k - g(X^k)X^k
			auto inputRow = inputs.row(j).data();
			MatrixXf input = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(inputRow, 1, inCols);
			//add bias
			cout << "--input--" << endl << input << endl << "---------------" << endl;
			input.conservativeResize(input.rows(), input.cols() + 1);
			input.col(input.cols() - 1).setOnes();
			cout << "--input--" << endl << input << endl << "---------------" << endl;

			auto output = outputs.row(j);
			auto guess = Classify(wei, int(weights.rows()), int(weights.cols()), in, int(input.rows()), int(input.cols()), false);
			float error = (output(0, 0) - guess) * learningRate;
			auto result = input * error;
			weights += result;
		}
	}

	float* weightsResult;
	weightsResult = new float[weights.rows()*weights.cols()];
	Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(weightsResult, weights.rows(), weights.cols()) = weights;
	return weightsResult;
}



//////


double* train(double* in, int inRows, int inCols, double* out, int outRows, int outCols)
{
	//convert double* to Matrix

	int size = (sizeof(in) / sizeof(*in));

	MatrixXd inputs = Map<MatrixXd>(in, inRows, inCols);
	MatrixXd outputs = Map<MatrixXd>(out, outRows, outRows);
	//add bias
	//cout << inputs << endl;

	inputs.conservativeResize(inputs.rows(), inputs.cols() + 1);
	inputs.col(inputs.cols() - 1).setOnes();
	//cout << inputs << endl;
	//apply pseudo inverse
	MatrixXd model = ((inputs.transpose()*inputs).inverse()*inputs.transpose())*outputs;
	//convert matrix to double*
	double *result;
	result = new double[model.rows()*model.cols()];
	Map<MatrixXd>(result, model.rows(), model.cols()) = model;
	return result;
}

double predict(double* mod, int modRows, int modCols, double* in, int inRows, int inCols)
{
	//convert
	MatrixXd model = Map<MatrixXd>(mod, modRows, modCols);
	MatrixXd input = Map<MatrixXd>(in, inRows, inCols);
	input.conservativeResize(input.rows(), input.cols() + 1);
	input.col(input.cols() - 1).setOnes();
	return (input * model)(0, 0);
}