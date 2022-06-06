#pragma once
#include <iostream>
#include <string>
#include <time.h>
#include <stdlib.h>
#include "Buffer.h"
#include "ExperienceContainer.h"
#include "NeuralNetwork.h"
#include <algorithm>
#include <vector>
#include <deque>
#include <string>
#include <fstream>
#include <sstream>
#include <tuple>
#include <sys/stat.h>
#include "DEFINED_VARS.h"

class DeepQ
{
private:
	// experience buffer
	std::deque<ExperienceContainer> experienceMemory;

	// main network
	std::shared_ptr<NeuralNetwork> mainNetwork;
	// target network
	std::shared_ptr<NeuralNetwork> targetNetwork;

	// stepCounter for experience replay
	unsigned long stepCounter = 0;	

	// exploration
	double exploration = EXPLORATION_DEFAULT;
	double minimumExploration = EXPLORATION_DEFAULT;
	double explorationDecay = EXPLORATION_DECAY;

	double gamma = GAMMA;
	unsigned int trainEpochs = TRAIN_FOR_EPOCHS;
	unsigned int updateTargetWeightsAfterSteps = UPDATE_TARGET_WEIGHTS_AFTER;
	unsigned int updateMainWeightsAfterSteps = UPDATE_TARGET_WEIGHTS_AFTER;
	unsigned int replayBatchSize = REPLAY_EXPERIENCE_BATCH_SIZE;
	unsigned int experienceMemorySize = EXPERIENCE_MEMORY_SIZE;

public:
	DeepQ(NeuralNetwork* neuralNetwork);
	~DeepQ();

	int getAction(std::vector<double> state);
	void remember(std::vector<double> initialState, int action, std::vector<double> reachedState, double reward, bool gameOver);
	void replayExperience();

	void setExploration(double value);
	void setMinimumExploration(double value);
	void setExplorationDecay(double value);

	void setGamma(double gamma);
	void setTrainEpochs(unsigned int steps);
	void setUpdateTargetWeightsAfterSteps(unsigned int steps);
	void setReplayBatchSize(unsigned int numberOfSamples);
	void setExperienceMemorySize(unsigned int size);

	void save(std::string path);

private:
	void synchronizeTargetNetwork();
	void checkPercentageValue(double value);
	bool explorePolicy(double probability);
	void updateExploration();
};