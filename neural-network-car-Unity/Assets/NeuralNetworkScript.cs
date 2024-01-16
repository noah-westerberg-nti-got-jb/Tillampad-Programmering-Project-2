using System.Collections;
using UnityEngine;

namespace Assets
{
    public class NeuralNetworkScript : MonoBehaviour
    {
        NeuralNetwork neuralNework = new NeuralNetwork();

        void Update()
        {
            
        }
    }

    class NeuralNode
    { 
        public double output;
        public double[] input, weights, biases;
    }
    class NetworkLayer
    {
        NeuralNode[] nodes;
    }

    class NeuralNetwork
    {

        NetworkLayer[] layers;

        public void Learn()
        {

        }
    }
}