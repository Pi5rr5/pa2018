from cffi import FFI
import numpy as np

ffi = FFI()

C = ffi.dlopen("../../part2/Unity_dll/x64/Debug/Unity_dll.dll")

ffi.cdef("""
    float* trainNaiveRBF(float*, int, int, float*, int, int, float);
    float naiveRBFRegression(float*, int, int, float*, int, int, float*, int, int, float)
""")


def as_pointer(numpy_array):
    return ffi.cast("float*", numpy_array.__array_interface__['data'][0])


# import data from csv
data = np.genfromtxt('inputs.csv', delimiter=",", skip_header=1, dtype=float)
# shuffle data set
np.random.shuffle(data)
# split inputs & outputs
inputs = data[0:, 0:20].astype('float32')
outputs = data[0:, 20:21].astype(int)
# split train and test data set
inputs_train = inputs[0:int(inputs.shape[0] * 0.8)]
outputs_train = outputs[0:int(outputs.shape[0] * 0.8)]

inputs_test = inputs[int(inputs.shape[0] * 0.8):inputs.shape[0]]
outputs_test = outputs[int(outputs.shape[0] * 0.8):outputs.shape[0]]

print("Training model ...")
gamma = 0.001
model = C.trainNaiveRBF(as_pointer(inputs_train), inputs_train.shape[0], inputs_train.shape[1],
                        as_pointer(outputs_train), outputs_train.shape[0], outputs_train.shape[1],
                        gamma)
precision = 0
for i, val in enumerate(inputs_test):
    res = C.naiveRBFRegression(as_pointer(inputs_train), inputs_train.shape[0], inputs_train.shape[1],
                               as_pointer(inputs_test[i]), 1, 20,
                               model, inputs_train.shape[0], inputs_train.shape[0],
                               gamma)
    if (round(res) > 0) == outputs_test[i]:
        precision += 1
print("precision after training = {:0.2f}%".format((precision/inputs_test.shape[0])*100))