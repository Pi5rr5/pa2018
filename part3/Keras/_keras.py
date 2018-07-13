import keras
from keras.metrics import binary_accuracy
from keras.models import Sequential
from keras.layers import Dense, Dropout
import numpy as np
from keras.optimizers import sgd, rmsprop, adam

data = np.genfromtxt('inputs_keras.csv', delimiter=",", skip_header=1, dtype=float)

np.random.shuffle(data)
inputs = data[0:, 0:20]
outputs = data[0:, 20:21].astype(int)

model = Sequential()
model.add(Dense(2048, input_dim=20, activation='tanh'))
#model.add(Dropout(0.5))
model.add(Dense(2048, activation='tanh'))
model.add(Dense(2048, activation='tanh'))
model.add(Dense(2048, activation='tanh'))
#model.add(Dropout(0.5))

# use relu and 4 layer 4096 to fit at 98%
# use adam as optimizer
# model.add(Dense(4095, activation='relu'))


model.add(Dense(1, activation='sigmoid'))

# tanh
model.compile(loss='binary_crossentropy', optimizer=sgd(0.01, momentum=0.9), metrics=[binary_accuracy])

# relu
#model.compile(loss='binary_crossentropy', optimizer=adam(), metrics=[binary_accuracy])

experiment_id = "4_layer_2048_neurones_10000_tanh/"
tb_callback = keras.callbacks.TensorBoard('./logs/' + experiment_id)

model.fit(inputs, outputs, epochs=10000, verbose=1, validation_split=0.2, batch_size=64, callbacks=[tb_callback])
