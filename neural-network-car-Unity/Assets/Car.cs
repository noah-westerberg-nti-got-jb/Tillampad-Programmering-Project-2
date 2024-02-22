using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

class Car : MonoBehaviour
{
    public NeuralNetwork network;
    CarController controller;
    Score score;
    public int index;

    public bool crashed = false;

    int viewLines;
    float viewAngle, viewDistance;

    public void Initialize(int index, int[] networkSize, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashScorePenalty, int rngSeed)
    {
        this.index = index;

        controller = gameObject.GetComponent<CarController>();
        controller.Initialize(carControllerValues);
        this.viewLines = viewLines;
        this.viewAngle = viewAngle;
        viewDistance = maxViewDistance;

        System.Random rng = new System.Random(rngSeed);
        network = new NeuralNetwork(networkSize, rng);

        score = gameObject.GetComponent<Score>();
        score.Initialize(scoreDistanceMultiplyer, crashScorePenalty);

        Vector3 colorValues = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        Color carColor = new Color(colorValues.x, colorValues.y, colorValues.z);
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            meshRenderer.material.color = carColor;
    }

    public void Crash()
    {
        crashed = true;
        score.Crash();
    }

    public float GetScore()
    {
        return score.GetScore();
    }

    [SerializeField] string currentlyDoing;

    public void Drive()
    {
        double[] distances = controller.GetDistances(viewLines, -viewAngle, viewAngle, viewDistance);
        double[] input = distances.Concat(new double[] { controller.velocity }).ToArray();
        double[] outputs = network.GetOutputs(input);
        int outputNode = network.CalculateOutputNode(outputs);
        switch (outputNode)
        {
            case 0:
                controller.Accelerate();
                currentlyDoing = "accelerate";
                break;
            case 1:
                controller.Break();
                currentlyDoing = "break";
                break;
            case 2:
                controller.Turn(1);
                currentlyDoing = "right";
                break;
            case 3:
                controller.Turn(-1);
                currentlyDoing = "left";
                break;
        }

        //foreach (NetworkLayer layer in network.GetLayers())
        //{
        //    Debug.Log("Weights: ");
        //    double[,] weights = layer.GetWeights();
        //    for (int i = 0; i < weights.GetLength(0); i++)
        //    {
        //        Debug.Log(i);
        //        for (int j = 0; j < weights.GetLength(1); j++)
        //        {
        //            Debug.Log(weights[i, j]);
        //        }
        //    }

        //    Debug.Log("Biases: ");
        //    double[] biases = layer.GetBiases();
        //    for (int i = 0; i < weights.GetLength(0); i++)
        //    {
        //        Debug.Log(i);
        //        Debug.Log(biases[i]);
        //    }
        //}
    }
}