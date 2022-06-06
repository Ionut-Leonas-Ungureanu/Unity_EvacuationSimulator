using System;
using System.Runtime.InteropServices;

namespace DeepQNetworkWrapper
{
    public class DeepQWrapper
    {
        private IntPtr _deepQ = IntPtr.Zero;

        /// <summary>
        /// Creates an instance of Deep Q Network
        /// </summary>
        /// <param name="inputSize"></param>
        /// <param name="outputSize"></param>
        /// <param name="numberOfHiddenLayers"></param>
        /// <param name="hiddenLayerSize"></param>
        /// <param name="function">An upper case string with the name of the activation function: SIGMOID, RELU</param>
        public DeepQWrapper(NeuralNetworkWrapper neuralNetwork)
        {
            _deepQ = CreateDeepQ(neuralNetwork.Pointer);
        }

        ~DeepQWrapper()
        {
            DestroyDeepQ(_deepQ);
            _deepQ = IntPtr.Zero;
        }

        /// <summary>
		/// Gets the next action based on current state. 
		/// </summary>
		/// <param name="state">State of the environment.</param>
		/// <param name="reward">Reward for the action that brought the environment in current state.</param>
		/// <returns>action</returns>
		public int GetAction(double[] state)
        {
            return DeepQGetAction(_deepQ, state, (uint)state.Length);
        }

        public void Remember(double[] initialState, int action, double[] reachedState, double reward, bool gameOver)
        {
            DeepQRemember(_deepQ, initialState, (uint)initialState.Length, action, reachedState, reward, gameOver);
        }

        public void ReplayExperience()
        {
            DeepQReplayExperience(_deepQ);
        }

        /// <summary>
        /// Set exploration value
        /// </summary>
        /// <param name="exploration">Double in [0, 1]</param>
        public void SetExploration(double exploration)
        {
            DeepQSetExploration(_deepQ, exploration);
        }

        /// <summary>
        /// Set minimum exploration value
        /// </summary>
        /// <param name="minimumExploration">Double in [0, 1]</param>
        public void SetMinimumExploration(double minimumExploration)
        {
            DeepQSetMinimumExploration(_deepQ, minimumExploration);
        }

        /// <summary>
        /// Set exploration decay value
        /// </summary>
        /// <param name="minimumExploration">Double in [0, 1]</param>
        public void SetExplorationDecay(double explorationDecay)
        {
            DeepQSetExplorationDecay(_deepQ, explorationDecay);
        }

        /// <summary>
        /// Set gamma value
        /// </summary>
        /// <param name="gamma">Double in [0, 1]</param>
        public void SetGamma(double gamma)
        {
            DeepQSetGamma(_deepQ, gamma);
        }

        /// <summary>
        /// Set after how many steps to update the target weights
        /// </summary>
        /// <param name="steps"></param>
        public void SetUpdateTargetWeightsAfterSteps(uint steps)
        {
            DeepQSetUpdateTargetWeightsAfterSteps(_deepQ, steps);
        }

        /// <summary>
        /// Set the size of experience batch to train
        /// </summary>
        /// <param name="numberOfSamples"></param>
        public void SetReplayBatchSize(uint numberOfSamples)
        {
            DeepQSetReplayBatchSize(_deepQ, numberOfSamples);
        }

        /// <summary>
        /// Set the size for experience buffer
        /// </summary>
        /// <param name="size"></param>
        public void SetExperienceMemorySize(uint size)
        {
            DeepQSetExperienceMemorySize(_deepQ, size);
        }

        /// <summary>
        /// Set if DQN should train or not
        /// </summary>
        /// <param name="shouldTrain">Boolean: "True" if it should train, otherwise [False]</param>
        public void SetTrainEpochs(uint epochs)
        {
            DeepQSetTrainEpochs(_deepQ, epochs);
        }

        public void SaveNetwork(string path)
        {
            DeepQSaveNetwork(_deepQ, path);
        }

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateDeepQ(IntPtr neuralNetwork);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyDeepQ(IntPtr dqnObject);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DeepQGetAction(IntPtr dqnObject, double[] state, uint stateSize);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DeepQRemember(IntPtr dqnObject, double[] initialState, uint stateSize, int action, double[] reachedState, double reward, bool gameOver);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern int DeepQReplayExperience(IntPtr dqnObject);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetExploration(IntPtr dqnObject, double exploration);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetMinimumExploration(IntPtr dqnObject, double minimumExploration);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetExplorationDecay(IntPtr dqnObject, double explorationDecay);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetGamma(IntPtr dqnObject, double gamma);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetUpdateTargetWeightsAfterSteps(IntPtr dqnObject, uint steps);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetReplayBatchSize(IntPtr dqnObject, uint numberOfSamples);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetExperienceMemorySize(IntPtr dqnObject, uint numberOfSamples);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSetTrainEpochs(IntPtr dqnObject, uint epochs);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DeepQSaveNetwork(IntPtr dqnObject, string path);
    }
}
