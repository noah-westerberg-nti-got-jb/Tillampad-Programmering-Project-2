using UnityEngine;

class NetworkLayer
{
    int inputNodesNum, outputNodesNum;
    double[,] weights;
    double[] biases;

    public NetworkLayer(int inputNodes, int outputNodes)
    {
        inputNodesNum = inputNodes;
        outputNodesNum = outputNodes;

        weights = new double[outputNodes, inputNodes];
        biases = new double[outputNodes];
    }

    public NetworkLayer(int outputNodes)
    {
        inputNodesNum = 1;
        outputNodesNum = outputNodes;

        weights = new double[outputNodes, 1];
        for (int i = 0; i < outputNodes; i++)
            weights[i, 0] = 1;
        biases = new double[outputNodes];
    }

    public double[] CalculateOutputs(double[] input)
    {

        double[] output = new double[outputNodesNum];

        for (int i = 0; i < outputNodesNum; i++)
        {
            for (int j = 0; j < inputNodesNum; j++)
            {
                output[i] += input[j] * weights[i, j];
            }
            output[i] += biases[i];
            output[i] = Activation(output[i]);
        }

        return output;
    }

    double Activation(double input)
    {
        return 1 / (1 + System.Math.Exp(-input));
    }

    public void SetWeights(int receivingNodeNum, double[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            weights[receivingNodeNum, i] = values[i];
        }
    }
    public void ShiftWeights(int receivingNodeNum, double[] values)
    {
        for (int i = 0; i < values.Length; i++)
            weights[receivingNodeNum, i] += values[i];
    }
    public double[,] GetWeights()
    {
        return weights;
    }

    public void SetBiases(double[] values)
    {
        biases = values;
    }
    public void ShiftBiases(double[] values)
    {
        for (int i = 0; i < biases.Length; i++)
            biases[i] += values[i];  
    }

    public double[] GetBiases()
    {
        return biases;
    }

}

class NeuralNetwork
{
    NetworkLayer[] layers;

    public double[] GetOutputs(double[] input)
    {
        foreach (NetworkLayer layer in layers)
            input = layer.CalculateOutputs(input);

        return input;
    }

    public int CalculateOutputNode(double[] outputValues)
    {
        int outputNode = 0;
        for (int i = 0; i < outputValues.Length; i++)
            if (outputValues[i] > outputValues[outputNode])
                outputNode = i;

        return outputNode;
    }

    public NeuralNetwork(int[] nodes, System.Random rng)
    {
        NetworkLayer[] newLayers = new NetworkLayer[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            if (i == 0)
            {
                newLayers[0] = new NetworkLayer(nodes[i]);
                continue;
            }

            newLayers[i] = new NetworkLayer(nodes[i - 1], nodes[i]);
            double[] biases = new double[nodes[i]];
            for (int j = 0; j < nodes[i]; j++)
            {
                biases[j] = rng.NextDouble();

                double[] weigths = new double[nodes[i - 1]];
                for (int k = 0; k < nodes[i - 1]; k++)
                {
                    weigths[k] = rng.NextDouble();
                }
                newLayers[i].SetWeights(j, weigths);
            }
            newLayers[i].SetBiases(biases);
        }

        layers = newLayers;
    }

    public NetworkLayer[] GetLayers()
    {
        return layers;
    }

    public void SetLayers(NetworkLayer[] newLayers)
    {
        layers = newLayers;
    }
}