import ctypes
import numpy as np

dll = ctypes.WinDLL("C:/Users/piesimon/Documents/esgi/pa2018/part2/Unity_dll/x64/Debug/Unity_dll.dll")

cFuncType = ctypes.WINFUNCTYPE(ctypes.c_int)
func = cFuncType(("return42", dll))

toto = func()

print(toto)

# import data from csv
data = np.genfromtxt('inputs.csv', delimiter=",", skip_header=1, dtype=float)

# shuffle data set
np.random.shuffle(data)

# split inputs & outputs
inputs = data[0:, 0:20]
outputs = data[0:, 20:21].astype(int)

# split train and test data set
input_train = inputs[slice(0, int(inputs.shape[0] * 0.8))]
input_test = inputs[slice(int(inputs.shape[0] * 0.8), inputs.shape[0])]
