#include "pch.h"
#include "DerivativeFunctions.h"

double DerivativeFunctions::linear(double x)
{
	return 1.0;
}

double DerivativeFunctions::relu(double x)
{
	return (x > 0) ? 1.0 : 0;
}

double DerivativeFunctions::selu(double x)
{
	double f = x > 0 ? 1 : SELU_ALPHA * std::exp(x);
	return SELU_LAMBDA * f;
}

double DerivativeFunctions::sigmoid(double x)
{
	return x * (1.0 - x);
}
