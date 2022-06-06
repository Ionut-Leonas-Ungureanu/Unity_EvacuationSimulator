#include "pch.h"
#include "ActivationFunctions.h"
#include <math.h>

double ActivationFunctions::linear(double x)
{
	return x;
}

double ActivationFunctions::relu(double x)
{
	return (x > 0) ? x : 0;
}

double ActivationFunctions::selu(double x)
{
	double f = x > 0 ? x : SELU_ALPHA * std::exp(x) - SELU_ALPHA;
	return SELU_LAMBDA * f;
}

double ActivationFunctions::sigmoid(double x)
{
	return (1.0 / (1.0 + exp(-x)));
}
