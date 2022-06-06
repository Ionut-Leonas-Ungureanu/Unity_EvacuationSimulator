#include "pch.h"
#include "Neuron.h"

Neuron::Neuron()
{
	// default
}

Neuron::Neuron(size_t weightsSize, double(*activation)(double), double(*derivative)(double))
{
	this->activationFunction = activation;
	this->derivativeActivationFunction = derivative;
	this->generateWeights(weightsSize);
}

Neuron& Neuron::operator=(Neuron neuron)
{
	this->activationFunction = neuron.activationFunction;
	this->derivativeActivationFunction = neuron.derivativeActivationFunction;

	this->weights.clear();
	for (auto weight : neuron.weights)
	{
		this->weights.push_back(weight);
	}
	this->biasWeight = neuron.biasWeight;

	return *this;
}

double Neuron::activate(std::vector<double> inputs)
{
	return this->activationFunction(this->sum(inputs));
}

double Neuron::derivative(double x)
{
	return this->derivativeActivationFunction(x);
}

fptr Neuron::getActivation()
{
	return this->activationFunction;
}

fptr Neuron::getDeactivation()
{
	return this->derivativeActivationFunction;
}

double Neuron::sum(std::vector<double> inputs)
{
	double sum = bias * biasWeight;
	for (size_t i = 0; i<inputs.size(); ++i)
	{
		sum += this->weights[i] * inputs[i];
	}
	return sum;
}

void Neuron::generateWeights(size_t numberWeights)
{
	// Set limits based on Xavier rule
	std::default_random_engine engine;
	std::uniform_real_distribution<double> distribution(-1.0 / sqrt(numberWeights), 1.0 / sqrt(numberWeights));

	// Generate random weights between LO and HI
	for (size_t i = 0; i < numberWeights; ++i)
	{
		this->weights.push_back(distribution(engine));
	}

	// Generate random weight for bias
	do
	{
		this->biasWeight = distribution(engine);
	} while (biasWeight == 0);
}
