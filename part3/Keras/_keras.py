import keras
from keras.metrics import binary_accuracy
from keras.models import Sequential
from keras.layers import Dense, Dropout
import numpy as np
from keras.optimizers import sgd, rmsprop, adam
from trainValTensorBoard import TrainValTensorBoard

data = np.genfromtxt('inputs_keras.csv', delimiter=",", skip_header=1, dtype=float)

np.random.shuffle(data)
inputs = data[0:, 0:20].astype('float32')
outputs = data[0:, 20:21].astype(int)

x_train = inputs[0: round(inputs.shape[0]*0.8)]
y_train = outputs[0: round(inputs.shape[0]*0.8)]
x_test = inputs[round(inputs.shape[0]*0.8): inputs.shape[0]]
y_test = outputs[round(inputs.shape[0]*0.8): inputs.shape[0]]

model = Sequential()
model.add(Dense(64, input_dim=20, activation='relu'))
model.add(Dropout(0.5))
model.add(Dense(64, activation='relu'))
model.add(Dropout(0.5))
model.add(Dense(1, activation='sigmoid'))

model.compile(loss='binary_crossentropy', optimizer=rmsprop(), metrics=[binary_accuracy])

model.fit(x_train, y_train,
          validation_data=(x_test, y_test),
          epochs=1000,
          batch_size=128,
          callbacks=[TrainValTensorBoard(write_graph=False)])
score = model.evaluate(x_test, y_test, verbose=1, batch_size=128)

unique, counts = np.unique(outputs, return_counts=True)
train_repartition = dict(zip(unique, counts))
print(train_repartition)

# use relu and 4 layer 4096 to fit at 98%
# use adam as optimizer
# model.add(Dense(1, activation='sigmoid'))
# tanh
#model.compile(loss='binary_crossentropy', optimizer=sgd(0.01, momentum=0.9), metrics=[binary_accuracy])
# relu
# model.compile(loss='binary_crossentropy', optimizer=adam(), metrics=[binary_accuracy])

# experiment_id = "4_layer_2_neurones_dropout_1000_tanh/"
# tb_callback = keras.callbacks.TensorBoard('./logs/' + experiment_id)

# model.fit(inputs, outputs, epochs=1000, verbose=1, validation_split=0.2, batch_size=6, callbacks=[tb_callback])

# test
#model.fit(inputs, outputs, epochs=10000, verbose=1, validation_split=0.2, batch_size=64)
