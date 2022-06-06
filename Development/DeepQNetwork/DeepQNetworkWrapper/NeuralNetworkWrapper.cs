using System;
using System.Runtime.InteropServices;

namespace DeepQNetworkWrapper
{
    public class NeuralNetworkWrapper
    {
        private IntPtr _neuralNetwork = IntPtr.Zero;
        public IntPtr Pointer => _neuralNetwork;

        public NeuralNetworkWrapper(double learningRate)
        {
            _neuralNetwork = createNeuralNetwork(learningRate);
        }

        ~NeuralNetworkWrapper()
        {
            destroyNeuralNetwork(_neuralNetwork);
            _neuralNetwork = IntPtr.Zero;
        }

        public void AddLayer(uint size, uint inputSize, string function)
        {
            addLayerToNeuralNetwork(_neuralNetwork, size, inputSize, function);
        }

        public void Save(string path)
        {
            saveNeuralNetwork(_neuralNetwork, path);
        }

        public void Load(string path)
        {
            loadNeuralNetwork(_neuralNetwork, path);
        }

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr createNeuralNetwork(double learningRate);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void destroyNeuralNetwork(IntPtr neuralNetwork);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void addLayerToNeuralNetwork(IntPtr neuralNetwork, uint size, uint inputSize, string function);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void saveNeuralNetwork(IntPtr neuralNetwork, string path);

        [DllImport("DeepQNetworkNative", CallingConvention = CallingConvention.Cdecl)]
        private static extern void loadNeuralNetwork(IntPtr neuralNetwork, string path);
    }
}
