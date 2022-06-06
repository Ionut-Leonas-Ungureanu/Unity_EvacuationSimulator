#include "pch.h"
#include "NeuralNetwork.h"
#include "ActivationFunctions.h"
#include "DerivativeFunctions.h"

NeuralNetwork::NeuralNetwork()
{
	this->learningRate = DEFAULT_LEARNING_RATE;
}

NeuralNetwork::NeuralNetwork(double learningRate)
{
	this->learningRate = learningRate;
}

NeuralNetwork::NeuralNetwork(const NeuralNetwork& network)
{
	this->learningRate = network.learningRate;
	this->layers.clear();
	this->layerFunctions.clear();

	for (size_t i = 0; i < network.layers.size(); ++i)
	{
		std::vector<std::shared_ptr<Neuron>> layer;
		for (size_t j = 0; j < network.layers[i].size(); ++j)
		{
			std::shared_ptr<Neuron> neuron(new Neuron(network.layers[i][j]->weights.size(), network.layers[i][j]->getActivation(), network.layers[i][j]->getDeactivation()));
			for (size_t w = 0; w < network.layers[i][j]->weights.size(); ++w)
			{
				neuron->weights[w] = network.layers[i][j]->weights[w];
			}
			neuron->biasWeight = network.layers[i][j]->biasWeight;
			layer.push_back(neuron);
		}
		this->layers.push_back(layer);
		this->layerFunctions.push_back(network.layerFunctions[i]);
	}
}

NeuralNetwork::~NeuralNetwork()
{
	// Destructor
}

void NeuralNetwork::addLayer(size_t size, size_t inputSize, std::string function)
{
	std::vector<std::shared_ptr<Neuron>> layer;

	auto neuralFunction = getNeuralFunction(function);

	for (size_t i = 0; i < size; ++i)
	{
		layer.push_back(std::shared_ptr<Neuron>(new Neuron(inputSize, neuralFunction.activationFunction, neuralFunction.derivativeActivationFunction)));
	}

	this->layerFunctions.push_back(function);
	this->layers.push_back(layer);
}

std::vector<double> NeuralNetwork::feedForward(std::vector<double> inputs)
{
	this->layersOutput.clear();
	this->layersOutput.push_back(inputs);

	for (auto layer : this->layers)
	{
		std::vector<double> layerOutputs;

		for (auto neuron : layer)
		{
			layerOutputs.push_back(neuron->activate(this->layersOutput.back()));
		}

		this->layersOutput.push_back(layerOutputs);
	}

	return this->layersOutput.back();
}

void NeuralNetwork::backPropagate(std::vector<double> expectedOutputs)
{
	std::vector<std::vector<double>> layersRo;

	// Calculate ro
	for (auto it = layers.rbegin(); it != layers.rend(); ++it)
	{
		std::vector<double> layerRo;

		if (it == layers.rbegin())
		{
			// Output layer
			for (size_t j = 0; j < (*it).size(); ++j)
			{
				auto neuron = (*it)[j];
				auto idx = this->layers.rend() - it;
				double ro = (expectedOutputs[j] - this->layersOutput[idx][j]) * neuron->derivative(this->layersOutput[idx][j]);
				layerRo.push_back(ro);
			}
		}
		else
		{
			// Hidden layer
			for (size_t j = 0; j < (*it).size(); ++j)
			{
				auto neuron = (*it)[j];

				auto previousLayerRo = layersRo.front();
				double previousRoSum = 0;
				for (size_t i = 0; i < previousLayerRo.size(); ++i)
				{
					previousRoSum += (*(it - 1))[i]->weights[j] * previousLayerRo[i];
				}

				double ro = previousRoSum * neuron->derivative(this->layersOutput[this->layers.rend() - it][j]);
				layerRo.push_back(ro);
			}
		}

		layersRo.insert(layersRo.begin(), layerRo);
	}

	// Update weights
	if (t == 0)
	{
		// initialize moments
		for (size_t i = 0; i < this->layers.size(); ++i)
		{
			std::vector<std::tuple<std::vector<double>, double>> mi, vi;
			for (size_t j = 0; j < this->layers[i].size(); ++j)
			{
				// moments for weights
				std::vector<double> mnWeights, vnWeights;
				for (size_t w = 0; w < this->layers[i][j]->weights.size(); ++w)
				{
					mnWeights.push_back(0);
					vnWeights.push_back(0);
				}
	
				// tuple ( weights moments , bias moment)
				mi.push_back(std::make_tuple(mnWeights, 0));
				vi.push_back(std::make_tuple(vnWeights, 0));
			}
			m.push_back(mi);
			v.push_back(vi);
		}
	}

	t += 1;
	// calculate moments
	for (size_t i = 0; i < layersRo.size(); ++i)
	{
		for (size_t j = 0; j < layersRo[i].size(); ++j)
		{
			// weights update
			for (size_t w = 0; w < this->layers[i][j]->weights.size(); ++w)
			{
				std::get<0>(m[i][j])[w] = beta1 * std::get<0>(m[i][j])[w] + (1.0 - beta1) * (layersRo[i][j] * this->layersOutput[i][w]);
				std::get<0>(v[i][j])[w] = beta2 * std::get<0>(v[i][j])[w] + (1.0 - beta2) * std::pow(layersRo[i][j] * this->layersOutput[i][w], 2);

				auto mPrime = std::get<0>(m[i][j])[w] / (1.0 - std::pow(beta1, t));
				auto vPrime = std::get<0>(v[i][j])[w] / (1.0 - std::pow(beta2, t));

				this->layers[i][j]->weights[w] += this->learningRate * mPrime / (std::sqrt(vPrime) + eps);
			}

			// bias update
			std::get<1>(m[i][j]) = beta1 * std::get<1>(m[i][j]) + (1.0 - beta1) * (layersRo[i][j] * this->layers[i][j]->bias);
			std::get<1>(v[i][j]) = beta2 * std::get<1>(v[i][j]) + (1.0 - beta2) * std::pow(layersRo[i][j] * this->layers[i][j]->bias, 2);

			auto mPrime = std::get<1>(m[i][j]) / (1.0 - std::pow(beta1, t));
			auto vPrime = std::get<1>(v[i][j]) / (1.0 - std::pow(beta2, t));

			this->layers[i][j]->biasWeight += this->learningRate * mPrime / (std::sqrt(vPrime) + eps);
		}
	}
}

void NeuralNetwork::copyWights(NeuralNetwork network)
{
	for (size_t i = 0; i < this->layers.size(); ++i)
	{
		for (size_t j = 0; j < this->layers[i].size(); ++j)
		{
			for (size_t w = 0; w < this->layers[i][j]->weights.size(); ++w)
			{
				this->layers[i][j]->weights[w] = network.layers[i][j]->weights[w];
			}
			this->layers[i][j]->biasWeight = network.layers[i][j]->biasWeight;
		}
	}
}

size_t NeuralNetwork::inputSize()
{
	return this->layers.front().front()->weights.size();
}

size_t NeuralNetwork::outputSize()
{
	return this->layers.back().size();
}

void NeuralNetwork::save(std::string path)
{
	rapidjson::Document d;
	d.Parse("{}");

	// Write learning rate
	d.AddMember(LEARNING_RATE_NODE, this->learningRate, d.GetAllocator());

	rapidjson::Value layers(rapidjson::kArrayType);
	// Parse layers
	for (size_t i = 0; i < this->layers.size(); ++i)
	{
		rapidjson::Value layer(rapidjson::kObjectType);
		layer.SetObject();
		rapidjson::Value neurons(rapidjson::kArrayType);

		// Write function type
		rapidjson::Value function(rapidjson::kStringType);
		function.SetString(this->layerFunctions[i].c_str(), this->layerFunctions[i].size(), d.GetAllocator());
		layer.AddMember(FUNCTION_NODE, function, d.GetAllocator());

		// Write size
		layer.AddMember(LAYER_SIZE, this->layers[i].size(), d.GetAllocator());

		// Write input size
		layer.AddMember(LAYER_INPUT_SIZE, this->layers[i][0]->weights.size(), d.GetAllocator());

		// Prepare neurons
		for (auto neuron : this->layers[i])
		{
			rapidjson::Value neuronObject(rapidjson::kObjectType);
			neuronObject.SetObject();

			// Write bias weight
			neuronObject.AddMember(BIAS_WEIGHT_NODE, neuron->biasWeight, d.GetAllocator());

			// Write input weights
			rapidjson::Value weights(rapidjson::kArrayType);
			for (auto weight : neuron->weights)
			{
				weights.PushBack(weight, d.GetAllocator());
			}
			neuronObject.AddMember(WEIGHTS_NODE, weights, d.GetAllocator());

			neurons.PushBack(neuronObject, d.GetAllocator());
		}

		// Add neurons to layer
		layer.AddMember(NEURONS_NODE, neurons, d.GetAllocator());

		layers.PushBack(layer, d.GetAllocator());
	}

	d.AddMember(LAYERS_NODE, layers, d.GetAllocator());

	// Write
	FILE* file;
	const char* filePath = path.c_str();
	fopen_s(&file, filePath, "wb");
	char *writeBuffer = new char[65536];
	rapidjson::FileWriteStream os(file, writeBuffer, sizeof(writeBuffer));
	rapidjson::Writer<rapidjson::FileWriteStream> writer(os);
	d.Accept(writer);

	// Close file
	if (file)
	{
		fclose(file);
	}
	// Free buffer
	delete[] writeBuffer;
}

void NeuralNetwork::load(std::string path)
{
	// Clear all layers
	this->layers.clear();
	this->layerFunctions.clear();

	struct stat info;

	int exists = stat(path.c_str(), &info);
	// File exists to load data from
	if (exists == 0)
	{
		std::ifstream in(path.c_str());
		std::stringstream buffer;
		buffer << in.rdbuf();

		rapidjson::Document d;
		d.Parse(&buffer.str()[0]);

		// Get epsilon and learning rate
		this->learningRate = d[LEARNING_RATE_NODE].GetDouble();

		// Get all weights
		const rapidjson::Value& layers = d[LAYERS_NODE];
		
		for (rapidjson::SizeType i = 0; i < layers.Size(); ++i)
		{
			// Get function
			std::string function(layers[i][FUNCTION_NODE].GetString());

			// Get size
			size_t size = layers[i][LAYER_SIZE].GetUint64();

			// Get input size
			size_t inputSize = layers[i][LAYER_INPUT_SIZE].GetUint64();

			this->addLayer(size, inputSize, function);

			const rapidjson::Value& neurons = layers[i][NEURONS_NODE].GetArray();

			for (rapidjson::SizeType n = 0; n < neurons.Size(); ++n)
			{
				const rapidjson::Value& neuron = neurons[n];
				std::vector<double> neuronWeights;

				// Get bias weight
				this->layers[i][n]->biasWeight = neuron[BIAS_WEIGHT_NODE].GetDouble();

				// Get weights
				const rapidjson::Value& weights = neuron[WEIGHTS_NODE].GetArray();

				// Create neuron

				// Fill weights
				for (rapidjson::SizeType w = 0; w < weights.Size(); ++w)
				{
					this->layers[i][n]->weights[w] = weights[w].GetDouble();
				}
			}
		}
	}
}

NeuralFunction NeuralNetwork::getNeuralFunction(std::string function)
{
	NeuralFunction neuralFunction;

	// Check if there is a mapping with the function string received
	if (ActivationFunctions::MappedActivationFunctionToString.find(function) == ActivationFunctions::MappedActivationFunctionToString.end())
	{
		// mapping does not exists
		function = "SIGMOID";
	}

	// Set activation function
	switch (ActivationFunctions::MappedActivationFunctionToString[function])
	{
	case ActivationFunctions::ActivationFunction::LINEAR:
		neuralFunction.activationFunction = ActivationFunctions::linear;
		neuralFunction.derivativeActivationFunction = DerivativeFunctions::linear;
		break;
	case ActivationFunctions::ActivationFunction::RELU:
		neuralFunction.activationFunction = ActivationFunctions::relu;
		neuralFunction.derivativeActivationFunction = DerivativeFunctions::relu;
		break;
	case ActivationFunctions::ActivationFunction::SELU:
		neuralFunction.activationFunction = ActivationFunctions::selu;
		neuralFunction.derivativeActivationFunction = DerivativeFunctions::selu;
		break;
	case ActivationFunctions::ActivationFunction::SIGMOID:
		neuralFunction.activationFunction = ActivationFunctions::sigmoid;
		neuralFunction.derivativeActivationFunction = DerivativeFunctions::sigmoid;
		break;
	}

	return neuralFunction;
}