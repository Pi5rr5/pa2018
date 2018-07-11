from keras.metrics import binary_accuracy
from keras.models import Sequential
from keras.layers import Dense
import numpy as np
from keras.optimizers import sgd, rmsprop, adam

data = np.genfromtxt('inputs.csv', delimiter=",", skip_header=1, dtype=float)

np.random.shuffle(data)
inputs = data[0:, 0:20]
# TODO outputs between -1 & 1
outputs = data[0:, 20:21].astype(int)

model = Sequential()
model.add(Dense(4096, input_dim=20, activation='tanh'))

# use relu and 4 layer to fit at 98%
# use adam as optimizer
# model.add(Dense(4096, activation='relu'))

model.add(Dense(1, activation='sigmoid'))
model.compile(loss='binary_crossentropy', optimizer=sgd(0.001, momentum=0.9), metrics=[binary_accuracy])

model.fit(inputs, outputs, epochs=200, verbose=1, validation_split=0.2, batch_size=16)
