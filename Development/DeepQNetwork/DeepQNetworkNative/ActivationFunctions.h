#pragma once
#include <map>
#include <string>

// SELU
#define SELU_ALPHA 1.6732632423543772848170429916717
#define SELU_LAMBDA 1.0507009873554804934193349852946

namespace ActivationFunctions
{
	enum class ActivationFunction
	{
		LINEAR,
		RELU,
		SELU,
		SIGMOID
	};

	static std::map<std::string, ActivationFunction> MappedActivationFunctionToString = {
		{"LINEAR", ActivationFunction::LINEAR},
		{"RELU", ActivationFunction::RELU},
		{"SELU", ActivationFunction::SELU },
		{"SIGMOID", ActivationFunction::SIGMOID}
	};

	double linear(double x);
	double relu(double x);
	double selu(double x);
	double sigmoid(double x);
}