#pragma once
#include "NeuralNetwork.h"

extern "C" __declspec(dllexport) NeuralNetwork * createNeuralNetwork(double learningRate);
extern "C" __declspec(dllexport) void destroyNeuralNetwork(NeuralNetwork * neuralNetwork);
extern "C" __declspec(dllexport) void addLayerToNeuralNetwork(NeuralNetwork* neuralNetwork, size_t size, size_t inputSize, char* function);
extern "C" __declspec(dllexport) void saveNeuralNetwork(NeuralNetwork* neuralNetwork, char* path);
extern "C" __declspec(dllexport) void loadNeuralNetwork(NeuralNetwork* neuralNetwork, char* path);