import numpy as np
from sklearn import linear_model

data = np.genfromtxt('inputs_keras.csv', delimiter=",", skip_header=1, dtype=float)
np.random.shuffle(data)
inputs = data[0:, 0:20].astype('float32')
outputs = data[0:, 20:21].astype(int)

inputs_train = inputs[0:int(inputs.shape[0] * 0.8)]
outputs_train = outputs[0:int(outputs.shape[0] * 0.8)]
outputs_train.shape = (outputs_train.shape[0],)

inputs_test = inputs[int(inputs.shape[0] * 0.8):inputs.shape[0]]
outputs_test = outputs[int(outputs.shape[0] * 0.8):outputs.shape[0]]
outputs_test.shape = (outputs_test.shape[0],)

clf = linear_model.SGDClassifier()
clf.fit(inputs_train, outputs_train)
precision = clf.score(inputs_test, outputs_test) * 100
print("Precision= {:0.2f}%".format(precision))
