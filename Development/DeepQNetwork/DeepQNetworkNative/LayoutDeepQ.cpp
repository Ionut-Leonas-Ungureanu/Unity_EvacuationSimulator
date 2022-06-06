#include "pch.h"
#include <iostream>
#include <vector>
#include "DeepQ.h"

DeepQ* CreateDeepQ(NeuralNetwork* neuralNetwork)
{
	return new DeepQ(neuralNetwork);
}

void DestroyDeepQ(DeepQ* object)
{
	if (object != NULL)
	{
		delete object;
		object = NULL;
	}
}

int DeepQGetAction(DeepQ* object, double state[], size_t stateSize)
{
	std::vector<double> stateVector;
	for (size_t i = 0; i < stateSize; ++i)
	{
		stateVector.push_back(state[i]);
	}

	return object->getAction(stateVector);
}

void DeepQRemember(DeepQ* object, double initialState[], size_t stateSize, int action, double reachedState[], double reward, bool gameOver)
{
	std::vector<double> initialStateVector, reachedStateVector;
	for (size_t i = 0; i < stateSize; ++i)
	{
		initialStateVector.push_back(initialState[i]);
		reachedStateVector.push_back(reachedState[i]);
	}

	object->remember(initialStateVector, action, reachedStateVector, reward, gameOver);
}

void DeepQReplayExperience(DeepQ* object)
{
	object->replayExperience();
}

void DeepQSetExploration(DeepQ* object, double value)
{
	object->setExploration(value);
}

void DeepQSetMinimumExploration(DeepQ* object, double value)
{
	object->setMinimumExploration(value);
}

void DeepQSetExplorationDecay(DeepQ* object, double value)
{
	object->setExplorationDecay(value);
}

void DeepQSetGamma(DeepQ* object, double gamma)
{
	object->setGamma(gamma);
}

void DeepQSetUpdateTargetWeightsAfterSteps(DeepQ* object, unsigned int steps)
{
	object->setUpdateTargetWeightsAfterSteps(steps);
}

void DeepQSetReplayBatchSize(DeepQ* object, unsigned int numberOfSamples)
{
	object->setReplayBatchSize(numberOfSamples);
}

void DeepQSetExperienceMemorySize(DeepQ* object, unsigned int size)
{
	object->setExperienceMemorySize(size);
}

void DeepQSetTrainEpochs(DeepQ* object, unsigned int epochs)
{
	object->setTrainEpochs(epochs);
}

void DeepQSaveNetwork(DeepQ* object, char* path)
{
	object->save(std::string(path));
}

