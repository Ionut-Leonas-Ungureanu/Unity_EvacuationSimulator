#pragma once

// Neural Network
#define DEFAULT_LEARNING_RATE 0.001
#define DECAY_LEARNING_RATE 0.9999999999

// Serialization
#define EPSILON_NODE "epsilon"
#define LEARNING_RATE_NODE "learning_rate"
#define LAYERS_NODE "layers"
#define NEURONS_NODE "neurons"
#define BIAS_WEIGHT_NODE "bias_weight"
#define FUNCTION_NODE "function"
#define LAYER_SIZE "size"
#define LAYER_INPUT_SIZE "inputSize"
#define WEIGHTS_NODE "weights"
#define FILE_PATH "NEURAL_NETWORK_DATA.json"

// Deep Q Learning
#define EXPLORATION_UPPER_LIMIT 1
#define EXPLORATION_DEFAULT 0
#define EXPLORATION_DECAY 0.99

#define EXPERIENCE_MEMORY_SIZE 1000
#define TRAIN_FOR_EPOCHS 8
#define REPLAY_EXPERIENCE_BATCH_SIZE 100
#define REPLAY_MIN_EXPERIENCE_SIZE 100
#define UPDATE_TARGET_WEIGHTS_AFTER 100

#define GAMMA 0.75