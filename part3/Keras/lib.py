from cffi import FFI
import numpy as np

ffi = FFI()

C = ffi.dlopen("../../part2/Unity_dll/x64/Debug/Unity_dll.dll")

ffi.cdef("""
    float* InitWeight(int);
    float Classify(float*, int, int, float*, int, int, bool);
    float* TrainPerceptron(float*, int, int, float*, int, int, float*, int, int, float, int);
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

model = C.InitWeight(20)
model = C.TrainPerceptron(model, 1, 21,
                          as_pointer(inputs_train), inputs_train.shape[0], inputs_train.shape[1],
                          as_pointer(outputs_train), outputs_train.shape[0], outputs_train.shape[1],
                          0.01, 100)
precision = 0
for i, val in enumerate(inputs_test):
    res = C.Classify(model, 1, 21, as_pointer(inputs_test[i]), 1, 20, 1)
    if round(res) == outputs_test[i]:
        precision += 1
print("precision after training = {:0.2f}%".format((precision/inputs_test.shape[0])*100))

# test pipeline python <=> dll c++
# X = np.array(
#     [0.0, 0.0,
#      4.0, 3.0,
#      3.4, 1.5,
#      2.5, 2.3,
#      3.5, 2.2,
#      0.6, 0.6,
#      0.3, 1.0,
#      1.0, 3.0,
#      2.3, 3.0,
#      0.4, 2.5,
#      0.2, 1.4]).astype('float32')
#
# Y = np.array([1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0, -1.0, -1.0, -1.0, -1.0]).astype('float32')
#
# model = C.InitWeight(2)
# model = C.TrainPerceptron(model, 1, 3,
#                           as_pointer(X), 11, 2,
#                           as_pointer(Y), 11, 1,
#                           0.1, 1)
# T = np.array([2.0, 0.75])
# print(C.Classify(model, 1, 3, as_pointer(T), 1, 2, 1))
