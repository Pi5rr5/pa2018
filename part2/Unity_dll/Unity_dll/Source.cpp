#include <iostream>
#include <Eigen/Dense>

using namespace std;
using namespace Eigen;

extern "C" {
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

				auto guess = Classify(wei, int(weights.rows()), int(weights.cols()), mInput, int(input.rows()), int(input.cols()), false);

				MatrixXf output = outputs.row(j);
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
}