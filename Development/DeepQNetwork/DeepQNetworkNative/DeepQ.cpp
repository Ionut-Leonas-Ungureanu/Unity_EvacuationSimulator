#include "pch.h"
#include "DeepQ.h"

DeepQ::DeepQ(NeuralNetwork* neuralNetwork)
{
	// srand Call for future randoms
	srand(static_cast<unsigned int>(time(NULL)));

	// Create the main network
	this->mainNetwork = std::shared_ptr<NeuralNetwork>(new NeuralNetwork(*neuralNetwork));
	this->targetNetwork = std::shared_ptr<NeuralNetwork>(new NeuralNetwork(*neuralNetwork));
}

DeepQ::~DeepQ()
{
}

void DeepQ::synchronizeTargetNetwork()
{
	this->targetNetwork->copyWights(*(this->mainNetwork));
}

void DeepQ::checkPercentageValue(double value)
{
	if (value < 0 && value > 1)
	{
		throw std::invalid_argument("Value must be between 0 and 1.");
	}
}

bool DeepQ::explorePolicy(double probability)
{
	bool shouldExplore = probability <= this->exploration;
	this->updateExploration();
	
	return shouldExplore;
}

void DeepQ::updateExploration()
{
	if (this->exploration > this->minimumExploration)
	{
		this->exploration *= this->explorationDecay;
	}
}

int DeepQ::getAction(std::vector<double> state)
{
	// Select action using epsilon greedy policy
	auto explorationProbability = static_cast<double>(rand() / (static_cast<double>(RAND_MAX / EXPLORATION_UPPER_LIMIT)));
	if (this->explorePolicy(explorationProbability))
	{
		// Chose a random action for exploration
		return rand() % mainNetwork->outputSize();
	}
	else
	{
		// Get a prediction using the network
		auto results = mainNetwork->feedForward(state);
		return std::max_element(results.begin(), results.end()) - results.begin();
	}
}

void DeepQ::remember(std::vector<double> initialState, int action, std::vector<double> reachedState, double reward, bool gameOver)
{
	if (this->experienceMemory.size() == this->experienceMemorySize)
	{
		this->experienceMemory.pop_front();
	}

	this->experienceMemory.push_back({ initialState, action, reachedState, reward, gameOver });
}

void DeepQ::replayExperience()
{
	// Check to see if there is experience
	if (experienceMemory.size() == 0)
	{
		return;
	}

	auto memorySize = replayBatchSize;
	if (experienceMemory.size() < replayBatchSize)
	{
		memorySize = experienceMemory.size();
	}

	// Generate indexes
	std::default_random_engine engine;
	std::uniform_int_distribution<size_t> distribution(0, experienceMemory.size() - 1);

	std::vector<int> indexes;
	for (size_t i = 0; i < memorySize; ++i)
	{
		int index;
		do
		{
			index = distribution(engine);
		} while (std::find(indexes.begin(), indexes.end(), index) != indexes.end());

		indexes.push_back(index);
	}

	std::vector<std::tuple<std::vector<double>, std::vector<double>>> batch;

	// Get batch of training data
	for (auto i : indexes)
	{
		auto mainActionValues = mainNetwork->feedForward(experienceMemory[i].initialState);
		auto targetActionValues = targetNetwork->feedForward(experienceMemory[i].reachedState);

		// Calculate temporal target
		auto max = *std::max_element(targetActionValues.begin(), targetActionValues.end());
		if (experienceMemory[i].gameOver)
		{
			mainActionValues[experienceMemory[i].action] = experienceMemory[i].reward;
		}
		else
		{
			mainActionValues[experienceMemory[i].action] = experienceMemory[i].reward + gamma * max;
		}

		batch.push_back(std::make_tuple(experienceMemory[i].initialState, mainActionValues));
	}

	// Train main network for a number of epochs
	for (size_t epoch = 0; epoch < this->trainEpochs; ++epoch)
	{
		for (auto data : batch)
		{
			mainNetwork->feedForward(std::get<0>(data));
			mainNetwork->backPropagate(std::get<1>(data));
		}
	}

	// Synchronize target
	if (this->stepCounter % this->updateTargetWeightsAfterSteps == 0)
	{
		this->synchronizeTargetNetwork();
	}

	this->stepCounter++;
}

void DeepQ::setExploration(double value)
{
	this->checkPercentageValue(value);
	this->exploration = value;
}

void DeepQ::setMinimumExploration(double value)
{
	this->checkPercentageValue(value);
	this->minimumExploration = value;
}

void DeepQ::setExplorationDecay(double value)
{
	this->checkPercentageValue(value);
	this->explorationDecay = value;
}

void DeepQ::setGamma(double gamma)
{
	this->checkPercentageValue(gamma);
	this->gamma = gamma;
}

void DeepQ::setTrainEpochs(unsigned int trainEpochs)
{
	this->trainEpochs = trainEpochs;
}

void DeepQ::setUpdateTargetWeightsAfterSteps(unsigned int steps)
{
	this->updateTargetWeightsAfterSteps = steps;
}

void DeepQ::setReplayBatchSize(unsigned int numberOfSamples)
{
	this->replayBatchSize = numberOfSamples;
}

void DeepQ::setExperienceMemorySize(unsigned int size)
{
	this->experienceMemorySize = size;
}

void DeepQ::save(std::string path)
{
	this->mainNetwork->save(path);
}
