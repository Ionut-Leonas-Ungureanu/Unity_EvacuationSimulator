using System;

namespace Assets.Scripts.Prefabs.Bot.States.Constants
{
    public static class Locals
    {
        #region Observation

        public static readonly int OBSERVATION_NUMBER_OF_RAYS = 21;
        public static readonly int OBSERVATION_NUMBER_LEVELS_OF_RAYS = 2;
        public static readonly int OBSERVATION_TOTAL_NUMBER_OF_RAYS = OBSERVATION_NUMBER_OF_RAYS * OBSERVATION_NUMBER_LEVELS_OF_RAYS;
        public static readonly int OBSERVATION_NUMBER_OF_DATA_READINGS_PER_RAY = 2; // Distance & Fire
        public static readonly int OBSERVATION_NUMBER_OF_DATA_READINGS_PER_LEVEL = OBSERVATION_NUMBER_OF_RAYS * OBSERVATION_NUMBER_OF_DATA_READINGS_PER_RAY;
        public static readonly int OBSERVATION_NUMBER_OF_RAY_DATA = OBSERVATION_NUMBER_OF_RAYS * OBSERVATION_NUMBER_LEVELS_OF_RAYS * OBSERVATION_NUMBER_OF_DATA_READINGS_PER_RAY;
        public static readonly float OBSERVATION_START_ANGLE_OF_RAYS = -100;
        public static readonly float OBSERVATION_OFFSET_ANGLE_OF_RAYS = 10;
        public static readonly float OBSERVATION_MAX_DISTANCE_OF_RAYS = 5;

        public static readonly int OBSERVATION_SIZE = OBSERVATION_NUMBER_OF_RAY_DATA + 8;
        public static readonly int OBSERVATION_ANGLE_FROM_GOAL_INDEX = OBSERVATION_NUMBER_OF_RAY_DATA;
        public static readonly int OBSERVATION_ANGLE_VELOCITY_INDEX = OBSERVATION_ANGLE_FROM_GOAL_INDEX + 1;
        public static readonly int OBSERVATION_DISTANCE_INDEX = OBSERVATION_ANGLE_VELOCITY_INDEX + 1;
        public static readonly int OBSERVATION_COLLISION_INDEX = OBSERVATION_DISTANCE_INDEX + 1;

        #endregion

        #region Action

        public static readonly int ACTION_SIZE = 5;
        public static readonly float[] DIRECTION_VALUES = { -1f, -0.7f, 0f, 0.7f, 1f};

        #endregion

        #region Deep Q Network

        public static readonly uint DQN_HIDDEN_NO_NEURONS = (uint)OBSERVATION_SIZE; //(uint)Math.Ceiling(Math.Sqrt(OBSERVATION_SIZE * ACTION_SIZE));
        public static readonly double DQN_LEARNING_RATE = 0.001;
        public static readonly string DQN_HIDDEN_LAYER_FUNCTION = "SELU";
        public static readonly string DQN_OUTPUT_LAYER_FUNCTION = "LINEAR";
        public static readonly double DQN_GAMMA = 0.99;
        public static readonly double DQN_EXPLORATION = 0.1;
        public static readonly double DQN_MINIMUM_EXPLORATION = 0.05;
        public static readonly double DQN_EXPLORATION_DECAY = 0.995;
        public static readonly uint DQN_TRAIN_EPOCHS = 1;
        public static readonly uint DQN_REPLAY_BATCH_SIZE = 50;
        public static readonly uint DQN_EXPERIENCE_MEMORY_SIZE = 3000;
        public static readonly uint DQN_UPDATE_TARGET_STEPS = 100;
        public static readonly string DQN_NETWORK_FILE = "Network_Files/Network_data.json";
        public static readonly string DQN_NETWORK_TRAINING_FILE = "Network_Files/Network_data_training.json";

        #endregion
    }
}
