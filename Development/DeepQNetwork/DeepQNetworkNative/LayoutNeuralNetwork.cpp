#include "pch.h"
#include "LayoutNeuralNetwork.h"

NeuralNetwork* createNeuralNetwork(double learningRate)
{
    return new NeuralNetwork(learningRate);
}

void destroyNeuralNetwork(NeuralNetwork* neuralNetwork)
{
    if (neuralNetwork != NULL)
    {
        delete neuralNetwork;
        neuralNetwork = NULL;
    }
}

void addLayerToNeuralNetwork(NeuralNetwork* neuralNetwork, size_t size, size_t inputSize, char* function)
{
    std::string functionString(function);
    neuralNetwork->addLayer(size, inputSize, functionString);
}

void saveNeuralNetwork(NeuralNetwork* neuralNetwork, char* path)
{
    std::string pathString(path);
    neuralNetwork->save(pathString);
}

void loadNeuralNetwork(NeuralNetwork* neuralNetwork, char* path)
{
    std::string pathString(path);
    neuralNetwork->load(pathString);
}
