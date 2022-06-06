#pragma once
#include <iostream>
#include <vector>
#include <memory>
#include <string>
#include <fstream>
#include <sstream>
#include <random>
#include <math.h>
#include "Neuron.h"
#include "DEFINED_VARS.h"

// Rapid json
#include "rapidjson/document.h"
#include "rapidjson/writer.h"
#include "rapidjson/stringbuffer.h"
#include "rapidjson/filewritestream.h"

struct NeuralFunction
{
	double (*activationFunction)(double) = NULL;
	double (*derivativeActivationFunction)(double) = NULL;
};

class NeuralNetwork
{
private:
	double learningRate = DEFAULT_LEARNING_RATE;

	std::vector<std::vector<std::shared_ptr<Neuron>>> layers;
	std::vector<std::string> layerFunctions;
	std::vector<std::vector<double>> layersOutput;

	// ADAM variables
	std::vector<std::vector<std::tuple<std::vector<double>, double>>> m;
	std::vector<std::vector<std::tuple<std::vector<double>, double>>> v;
	double beta1 = 0.9, beta2 = 0.999, eps = 1e-8, eta = 0.01;
	unsigned long long t = 0;

public:
	NeuralNetwork();
	NeuralNetwork(double learningRate);
	NeuralNetwork(const NeuralNetwork& network);
	~NeuralNetwork();

	void addLayer(size_t size, size_t inputSize, std::string function);
	std::vector<double> feedForward(std::vector<double> inputs);
	void backPropagate(std::vector<double> expectedOutputs);
	void copyWights(NeuralNetwork network);
	size_t inputSize();
	size_t outputSize();
	void save(std::string path);
	void load(std::string path);

private:
	NeuralFunction getNeuralFunction(std::string function);
};

