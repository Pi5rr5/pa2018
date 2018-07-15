import keras
from keras.metrics import binary_accuracy
from keras.models import Sequential
from keras.layers import Dense, Dropout, Activation
import numpy as np
from keras.optimizers import sgd, rmsprop, adam
from keras.utils import get_custom_objects


def sign(x):
    return x > 0

get_custom_objects().update({'sign': Activation(sign)})

model = Sequential()
model.add(Dense(1, input_dim=20, output_dim=1, activation="tanh"))
model.add(Dropout(0.5))
# model.add(Dense(output_dim=1, activation='sigmoid'))
model.compile(loss='binary_crossentropy', optimizer=adam(), metrics=[binary_accuracy])

# Train
data = np.genfromtxt('inputs_keras.csv', delimiter=",", skip_header=1, dtype=float)
np.random.shuffle(data)
inputs = data[0:, 0:20].astype('float32')
outputs = data[0:, 20:21].astype(int)

experiment_id = "linear_classif_adam_binary_crossentropy_Dropout/"
tb_callback = keras.callbacks.TensorBoard('./logs/' + experiment_id)

# output logs
#model.fit(inputs, outputs, nb_epoch=5000, verbose=1, validation_split=0.2, callbacks=[tb_callback])

# test
model.fit(inputs, outputs, epochs=2000, verbose=1, validation_split=0.2)
