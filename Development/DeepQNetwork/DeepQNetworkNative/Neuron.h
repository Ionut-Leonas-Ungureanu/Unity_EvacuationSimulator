#pragma once
#include <iostream>
#include <vector>
#include <math.h>
#include <random>

typedef double (*fptr)(double);

class Neuron
{
protected:
	double (*activationFunction)(double) = NULL;
	double (*derivativeActivationFunction)(double) = NULL;

public:
	std::vector<double> weights;
	double bias = 1;
	double biasWeight = 0;

	Neuron();
	Neuron(size_t weightsSize, double (*activation)(double), double (*derivative)(double));
	Neuron& operator=(Neuron neuron);
	double activate(std::vector<double> inputs);
	double derivative(double x);
	fptr getActivation();
	fptr getDeactivation();

protected:
	virtual double sum(std::vector<double> inputs);
	virtual void generateWeights(size_t numberWeights);
};