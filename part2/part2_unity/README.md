# Unity folder

## :book: cheat sheet dll c++ :arrows_counterclockwise: Unity c#

### Import c++ dll

```csharp
[DllImport("dll_file_name")]
private static extern <type> <function>();
```

### Manage c++ function returning a pointer

```cpp
__declspec(dllexport) double* returnArray()
{
    auto arr = new double[4]{ 1,2,3,4 };
    return arr;
}
```

```cpp
double[] managedArray = new double[size];
Marshal.Copy(ptr, managedArray, 0, size);
```

### Pass Csharp array to c++

```cpp
__declspec(dllexport) double returnArraySum(double* arr, int size)
{
    double res = 0;
    for (auto it = 0; it < size; it++) {
        res += arr[it];
    }
    return res;
}
```

```csharp
[DllImport("dll_file_name")]
private static extern double returnArraySum(double[] array, int size);
//or
private static extern double returnArraySum(IntPtr array, int size);
...
```

```csharp
...
double[] managedArray = { 0.1, 0.2, 0.3, 0.4 };
//pass the managedArray directly
Debug.Log(returnArraySum(managedArray, 4));
//or
//allocate memory and copy the managedArray into IntPtr variable
int size = Marshal.SizeOf(managedArray[0]) * managedArray.Length;
IntPtr pnt = Marshal.AllocHGlobal(size);
Marshal.Copy(managedArray, 0, pnt, managedArray.Length);
Debug.Log(returnArraySum(pnt, 4));
```

## Free c++ array

```cpp
__declspec(dllexport) void releaseArray(double* arr)
{
    delete[] arr;
}
```