#include <iostream>
#include <Eigen/Dense>
#include <cstdlib>
#include <ctime>

using namespace std;
using namespace Eigen;

extern "C" {
	// RETURN 42 //
	__declspec(dllexport) int return42()
	{
		return 42;
	}


	// LINEAR REGRESSION //


	__declspec(dllexport) float* Train(float* in, int inRows, int inCols, float* out, int outRows, int outCols)
	{
		//convert float* to Matrix
		MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
		MatrixXf outputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(out, outRows, outCols);
		//add bias
		inputs.conservativeResize(inputs.rows(), inputs.cols() + 1);
		inputs.col(inputs.cols() - 1).setOnes();
		//apply pseudo inverse
		MatrixXf model = ((inputs.transpose()*inputs).ldlt().solve(inputs.transpose()))*outputs;
		//convert matrix to float*
		float *result;
		result = new float[model.rows()*model.cols()];
		Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(result, model.rows(), model.cols()) = model;
		return result;
	}

	__declspec(dllexport) float Predict(float* mod, int modRows, int modCols, float* in, int inRows, int inCols)
	{
		//convert
		MatrixXf model = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(mod, modRows, modCols);
		MatrixXf input = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
		input.conservativeResize(input.rows(), input.cols() + 1);
		input.col(input.cols() - 1).setOnes();
		return (input * model)(0, 0);
	}


	// CLASSIFICATION //


	__declspec(dllexport) float* InitWeight(int dimension)
	{
		float* weight;
		weight = new float[dimension + 1];
		// init weight between -1.0f & 1.0f
		srand((int)time(0));
		for (int i = 0; i < dimension + 1; i++)
			weight[i] = (rand() / (RAND_MAX / (2.0f))) - 1.0f;
		return weight;
	}

	__declspec(dllexport) float Classify(float* wei, int weiRows, int weiCols, float* in, int inRows, int inCols, bool addBias)
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

	__declspec(dllexport) float* TrainPerceptron(float* wei, int weiRows, int weiCols, float* in, int inRows, int inCols, float* out, int outRows, int outCols, float learningRate, int epoch)
	{
		MatrixXf weights = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weiRows, weiCols);
		MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
		MatrixXf outputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(out, outRows, outCols);
		for (int i = 0; i < epoch; i++)
		{
			for (int j = 0; j < inRows; j++)
			{
				//W += a(Y^k - g(X^k)X^k
				MatrixXf input = inputs.row(j);

				//add bias and map as float*
				input.conservativeResize(input.rows(), input.cols() + 1);
				input.col(input.cols() - 1).setOnes();
				float* mInput;
				mInput = new float[input.rows() * input.cols()];
				Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(mInput, input.rows(), input.cols()) = input;

				float guess = Classify(wei, int(weights.rows()), int(weights.cols()), mInput, int(input.rows()), int(input.cols()), false);

				MatrixXf output = outputs.row(j);
				float error = (output(0, 0) - guess) * learningRate;

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


	// RBF //


	__declspec(dllexport) float* findPhi(float* in, int inRows, int inCols, float gamma)
	{
		MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
		MatrixXf phi = MatrixXf::Zero(inCols, inCols);
		for(int i = 0; i < phi.rows(); i++)
		{
			for (int j = 0; j < phi.cols(); j++)
			{
				float normeRes = pow((inputs.col(i) - inputs.col(j)).norm(), 2);
				phi(i, j) = exp(-gamma * normeRes);
			}
		}
		float* phiResult;
		phiResult = new float[phi.rows()*phi.cols()];
		Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(phiResult, phi.rows(), phi.cols()) = phi;
		return phiResult;
	}

	__declspec(dllexport) float* trainNaiveRBF(float* in, int inRows, int inCols, float* out, int outRows, int outCols, float gamma)
	{
		MatrixXf outputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(out, outRows, outCols);
		float* tmp = findPhi(in, inRows, inCols, gamma);
		MatrixXf phi = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(tmp, inCols, inCols);
		MatrixXf res = phi.ldlt().solve(outputs);
		float* trainResult;
		trainResult = new float[res.rows()*res.cols()];
		Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(trainResult, res.rows(), res.cols()) = res;
		return trainResult;
	}


	__declspec(dllexport) float naiveRBFRegression(float* in, int inRows, int inCols, float* pre, int preRows, int preCols, float* wei, int weiRows, int weiCols, float gamma)
	{
		MatrixXf inputs = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(in, inRows, inCols);
		MatrixXf predict = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(pre, preRows, preCols);
		MatrixXf weights = Map<Matrix<float, Dynamic, Dynamic, RowMajor> >(wei, weiRows, weiCols);
		MatrixXf sum = MatrixXf::Zero(weiRows, weiCols);
		for (int i = 0; i < inCols; i++)
		{
			float normeRes = pow((predict - inputs.col(i)).norm(), 2);
			sum.row(i) = weights.row(i) * exp(-gamma * normeRes);
		}
		return sum.colwise().sum()(0,0);
	}
}