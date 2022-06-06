#pragma once
#include "DeepQ.h"

extern "C" __declspec(dllexport) DeepQ * CreateDeepQ(NeuralNetwork* neuralNetwork);
extern "C" __declspec(dllexport) void DestroyDeepQ(DeepQ * object);
extern "C" __declspec(dllexport) int DeepQGetAction(DeepQ * object, double state[], size_t stateSize);
extern "C" __declspec(dllexport) void DeepQRemember(DeepQ * object, double initialState[], size_t stateSize, int action, double reachedState[], double reward, bool gameOver);
extern "C" __declspec(dllexport) void DeepQReplayExperience(DeepQ * object);
extern "C" __declspec(dllexport) void DeepQSetExploration(DeepQ * object, double value);
extern "C" __declspec(dllexport) void DeepQSetMinimumExploration(DeepQ * object, double value);
extern "C" __declspec(dllexport) void DeepQSetExplorationDecay(DeepQ * object, double value);
extern "C" __declspec(dllexport) void DeepQSetGamma(DeepQ * object, double gamma);
extern "C" __declspec(dllexport) void DeepQSetUpdateTargetWeightsAfterSteps(DeepQ * object, unsigned int steps);
extern "C" __declspec(dllexport) void DeepQSetReplayBatchSize(DeepQ * object, unsigned int numberOfSamples);
extern "C" __declspec(dllexport) void DeepQSetExperienceMemorySize(DeepQ * object, unsigned int size);
extern "C" __declspec(dllexport) void DeepQSetTrainEpochs(DeepQ * object, unsigned int epochs);
extern "C" __declspec(dllexport) void DeepQSaveNetwork(DeepQ * object, char* path);