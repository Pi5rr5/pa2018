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



float* findPhi(float* in, int inRows, int inCols, float gamma)
{
	MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);

	MatrixXf phi = MatrixXf::Zero(inRows, inRows);
	/*
	cout << "inputs" << endl << inputs << endl << "****************************" << endl;
	cout << "find phi result:" << endl << phi << endl << "****************************" << endl;
	cout << "shape =" << endl << phi.rows()*phi.cols() << endl << "****************************" << endl;
	*/

	for(int i = 0; i < phi.rows(); i++)
	{
		for (int j = 0; j < phi.cols(); j++)
		{
			//|| {a,b} , {c,d} ||
			// (a-c)^2 + (b-d)^2
			//cout << i << " : " << inputs.col(i) << " - " << j << " : " << inputs.col(j) << endl;
			//cout << "---------------------" << endl;
			//cout << (inputs.col(i) - inputs.col(j)) << endl;

			//float normeRes = pow((inputs.col(i) - inputs.col(j))(0,0), 2) + pow((inputs.col(i) - inputs.col(j))(0, 0), 2);
			//cout << inputs.col(i) << " | " << inputs.col(j) << endl;
			//cout << pow((inputs.col(i) - inputs.col(j)).norm(), 2) << endl;
			float normeRes = pow((inputs.row(i) - inputs.row(j)).norm(), 2);
			phi(i, j) = exp(-gamma * normeRes);
		}
	}
	
	//cout << "find phi result:" << endl << phi << endl << "****************************" << endl;

	float* phiResult;
	phiResult = new float[phi.rows()*phi.cols()];
	Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(phiResult, phi.cols(), phi.cols()) = phi;
	return phiResult;
}

float* trainNaiveRBF(float* in, int inRows, int inCols, float* out, int outRows, int outCols, float gamma)
{
	MatrixXf outputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(out, outRows, outCols);
	cout << "outputs" << endl << outputs << endl << "****************************" << endl;

	MatrixXf phi = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(findPhi(in, inRows, inCols, gamma), inRows, inRows);
	cout << "phi" << endl << phi << endl << "****************************" << endl;


	MatrixXf res = phi.ldlt().solve(outputs.cast<float>());
	cout << "train resul: " << endl << res << endl << "****************************" << endl;
	float* trainResult;
	cout << res.rows() << " " << inRows << endl;
	trainResult = new float[res.rows()*res.cols()];
	Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(trainResult, res.rows(), res.cols()) = res;
	return trainResult;
}

float naiveRBFRegression(float* in, int inRows, int inCols, float* pre, int preRows, int preCols, float* wei, int weiRows, int weiCols, float gamma)
{
	MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
	MatrixXf predict = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(pre, preRows, preCols);
	MatrixXf weights = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weiRows, weiCols);
	
	/*
	cout << "== naiveRBFRegression ==" << endl;
	cout << endl << "----inputs-------" << endl << inputs << endl << "---------------" << endl;
	cout << endl << "----predict-------" << endl << predict << endl << "---------------" << endl;
	*/
	cout << endl << "----weights-------" << endl << weights << endl << "---------------" << endl;
	cout << "-------------------------------------------" << endl << endl;
	

	MatrixXf sum = MatrixXf::Zero(weiRows, weiCols);
	for (int i = 0; i < inRows; i++)
	{
		//cout << i << " : " << inputs.col(i) << " - " << predict << endl;
		
		//pow((inputs.col(i) - inputs.col(j)).norm(), 2);
		//float normeRes = pow((predict(0, 0) - inputs.row(i)(0, 0)), 2) + pow((predict(0, 1) - inputs.row(i)(0, 1)), 2);
		float normeRes = pow((predict - inputs.row(i)).norm(), 2);
		sum.row(i) = weights.row(i) * exp(-gamma * normeRes);
	}
	return sum.colwise().sum()(0,0);
}

// K

float* initialize_centroids(float* in, int inRows, int inCols, int qte)
{
	float* centroids = new float[qte];
	for (int i = 0; i < qte; i++)
		centroids[i] = -1;
	if (qte > inRows)
		return centroids;
	//srand(time(NULL));
	for (int i = 0; i < qte; i++)
	{
		bool isNew = true;
		do {
			isNew = true;
			srand((int)time(0));
			int current_rand = rand() % inRows;
			cout << "current_rand = " << current_rand << endl;
			for (int j = 0; j < i; i++)
			{
				if (current_rand == centroids[j])
				{
					isNew = false;
				}
			}
			if (isNew)
			{
				centroids[i] = current_rand;
			}
		} while (!isNew);
	}
	return centroids;
}


int* get_centroids_distances_indexes(float* in, int inRows, int inCols, float* cen, int cenRows, int cenCols)
{
	MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, 4, 2);
	MatrixXf centroids = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(cen, 2, 2);
	int* idx_array = new int[inRows];
	for (int i = 0; i < inputs.rows(); i++)
	{
		float _min = (inputs.row(i) - centroids.row(0)).norm();
		int _min_idx = 0;
		cout << "INPUT = " << inputs.row(i) << endl;
		for (int j = 0; j < centroids.rows(); j++)
		{
			cout << "CENTROID = " << centroids.row(j) << endl;
			auto norm = (inputs.row(i) - centroids.row(j)).norm();
			cout << "NORM = " << norm << endl;
			if (norm < _min)
			{
				_min = norm;
				_min_idx = j;
			}
		}
		cout << "MIN NORM = " << _min << "IDX = " << _min_idx << endl;
		idx_array[i] = _min_idx;
		cout << endl;
	}
	return idx_array;
}


int main()
{

	auto inputs = new float[26]
	{ 3.325f, -1.08f, 2.19f, 2.03f, 4.97f, 3.04f, 1.32f, 4.68f, 4.94f, 0.7799997f, 3.49f, 5.255004f, -1.07f, 2.27f, -1.38f, -0.06999922f, 1.56f, 3.23f, 0.4800014f, 1.21f, 2.31f, 0.42f, -1.449999f, -2.27f, 0.2800007f, -1.659999f };
	auto outputs = new int[13]
	{ -1, -1, -1, -1, -1, -1, 1, 1, 1, 1, 1, 1, 1 };

	auto predict = new float[2]{ -2.94f, 1.21f };
	float gamma = 0.1f;
	float* weights = trainNaiveRBF(inputs, 13, 2, outputs, 13, 1, gamma);
	float pred = naiveRBFRegression(inputs, 13, 2, predict, 1, 2, weights, 13, 1, gamma);
	cout << "predict result = " << pred << endl;


	/*
	auto in = new float[8]{ 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.75f, 0.75f, 1.0f };
	auto cen = new float[4]{ 1.0f, 1.0f, 0.75f, 1.0f };


	float* test = initialize_centroids(in, 4, 2, 2);
	for (int i = 0; i < 2; i++)
		cout << test[i] << " | ";


	int* idx_array = get_centroids_distances_indexes(in, 4, 2, cen, 2, 2);
	for (int i = 0; i < 4; i++)
		cout << idx_array[i] << " | ";
	*/
	/*
	auto inputs = new float[3]
	{ 0.0f, 1.0f, 2.0f };
	auto outputs = new float[3]{ 2.0f, 5.0f, 8.0f };

	auto predict = new float[1]{ 4.0f };
	float gamma = 0.1;
	float* weights = trainNaiveRBF(inputs, 1, 3, outputs, 3, 1, gamma);
	float pred = naiveRBFRegression(inputs, 1, 3, predict, 1, 1, weights, 3, 1, gamma);
	cout << "predict result = " << pred << endl;
	*/

	/*classif test
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
	*/
	return 0;
}

