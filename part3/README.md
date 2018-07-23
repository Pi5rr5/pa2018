# RBF, SVM and Kernel Machines

## tl;dr

### Lib implementation

* RBF Basique [code](../part2/Unity_dll/Unity_dll/Source.cpp#L130)
* Linear classification [notebook](Keras/LibLinearClassification.ipynb)
* RBF avec k means [code](../part2/ConsoleApplication/ConsoleApplication/ConsoleApplication.cpp#L168)

### Framework Machine Learning

* MLP binary classification [notebook](Keras/KerasMLPClassification.ipynb)
* Naive RBF [notebook](Keras/LibNaiveRBFClassification.ipynb)

## Dataset introduction

Gender Recognition by Voice and Speech Analysis. This database was created to identify a voice as male or female, based upon acoustic properties of the voice and speech. The dataset consists of 3,168 recorded voice samples, collected from male and female speakers.

> https://www.kaggle.com/primaryobjects/voicegender/home

## Dataset exploration

The dataset's shape is (3168, 21), the target label of this classification is in "label" column and others are features. These two categories ("male", "female") are just half and half. It seems no skew in this data set. I modified "label" column by "isMale" and the two categories by a binary (1, 0) corresponding to male and female.
[More analysis](Keras/PlotDataset.ipynb)

## Model evaluation

The 20 features will be used to predict the gender of the person's voice.