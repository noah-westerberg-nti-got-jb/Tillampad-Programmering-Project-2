using System.Collections;
using UnityEngine;

namespace Assets
{
    class NetworkLayer
    {
        double[,] weights;
        double[] biases;

        public NetworkLayer(int inputNodes, int outputNodes)
        {
            weights = new double[outputNodes, inputNodes];
            biases = new double[outputNodes];
        }

        public double[] calculateOutput(double[] input)
        {

            return new double[input.Length];
        }
    }

    class NeuralNetwork
    {
        NetworkLayer[] layers;

        public void Learn()
        {

        }
    }
}