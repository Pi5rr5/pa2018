import keras
import numpy as np

model = keras.models.Sequential()
model.add(keras.layers.Dense(2, input_shape=(2,), activation=keras.activations.tanh))
model.add(keras.layers.Dense(1, activation=keras.activations.tanh))
model.compile(optimizer=keras.optimizers.sgd(lr=0.1), loss=keras.losses.mae)

X = np.array([
    [1, 1],
    [-1, -1],
    [1, -1],
    [-1, 1]
])

Y = np.array([
    [1],
    [1],
    [-1],
    [-1]
])

model.fit(X, Y, epochs=1000)
model.predict(X)