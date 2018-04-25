extern "C" {

	__declspec(dllexport) double* returnArray()
	{
		auto arr = new double[4]{ 1,2,3,4 };
		return arr;
	}

	__declspec(dllexport) double returnArraySum(double* arr, int size)
	{	
		double res = 0;
		for (auto it = 0; it < size; it++) {
			res += arr[it];
		}
		return res;
	}

	__declspec(dllexport) void releaseArray(double* arr)
	{
		delete[] arr;
	}

}