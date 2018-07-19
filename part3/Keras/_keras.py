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
model.add(Dense(20, input_dim=20, activation='tanh'))
model.add(Dense(1, activation='sigmoid'))
model.compile(loss='binary_crossentropy', optimizer=adam(), metrics=[binary_accuracy])

model.fit(x_train, y_train,
          validation_data=(x_test, y_test),
          epochs=500,
          batch_size=96,
          callbacks=[TrainValTensorBoard(write_graph=False)])
score = model.evaluate(x_test, y_test, verbose=1, batch_size=96)

unique, counts = np.unique(outputs, return_counts=True)
train_repartition = dict(zip(unique, counts))
print(train_repartition)
