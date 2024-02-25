using System;
using System.Linq;
using UnityEngine;

[Serializable]
class Car : MonoBehaviour
{
    public NeuralNetwork network;
    [SerializeField, HideInInspector] CarController controller;
    [SerializeField, HideInInspector] Score score;
    public int index;

    float scoreShift = 0;

    [SerializeField] float currentScore;

    public bool crashed = false;

    [SerializeField, HideInInspector] int viewLines;
    [SerializeField, HideInInspector] float viewAngle, viewDistance;

    public System.Random rng;
    [SerializeField /* , HideInInspector */] int rngSeed;

    [SerializeField, HideInInspector] CarControllerArgs carControllerValues;
    [SerializeField, HideInInspector] float scoreDistanceMultiplyer, crashScorePenalty;

    public void Initialize(int index, int[] networkSize, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashScorePenalty, int rngSeed)
    {
        this.index = index;
        // name = "Car: " + index.ToString();

        controller = gameObject.GetComponent<CarController>();
        controller.Initialize(carControllerValues);
        this.carControllerValues = carControllerValues;
        this.viewLines = viewLines;
        this.viewAngle = viewAngle;
        viewDistance = maxViewDistance;

        this.rngSeed = rngSeed;
        rng = new System.Random(rngSeed + index);
        network = new NeuralNetwork(networkSize, rng);

        score = gameObject.GetComponent<Score>();
        score.Initialize(scoreDistanceMultiplyer, crashScorePenalty);
        this.scoreDistanceMultiplyer = scoreDistanceMultiplyer;
        this.crashScorePenalty = crashScorePenalty;
    }

    public void Crash()
    {
        crashed = true;
        score.Crash();
        controller.velocity = 0;
        scoreShift = 0;
    }

    public void StartScore()
    {
        score.StartScore();
    }

    public float GetScore()
    {
        return score.GetScore();
    }

    public float ScoreShift()
    {
        if (scoreShift == 0)
            scoreShift = (((float)rng.NextDouble() * 2) - 1);

        return scoreShift;
    }

    [SerializeField] string currentlyDoing;

    double Map(double valueIn, double inMin, double inMax, double outMin, double outMax)
    {
        return (valueIn - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    public void Drive()
    {
        currentScore = score.GetScore();

        double[] distances = controller.GetDistances(viewLines, -viewAngle, viewAngle, viewDistance);
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = Map(distances[i], 0, viewDistance, -1, 1);
        }
        double[] input = distances.Concat(new double[] { Map(controller.velocity, 0, carControllerValues.maxSpeed, -1, 1) }).ToArray();
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

    public Car Copy(GameObject newCarObject)
    {
        var serialized = JsonUtility.ToJson(this);
        // Debug.Log(serialized);
        JsonUtility.FromJsonOverwrite(serialized, newCarObject.GetComponent<Car>());
        return newCarObject.GetComponent<Car>();
    }

    public void SetRNG(int rng)
    {
        rngSeed = rng;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    void Start()
    {
        name = "Car: " + index.ToString();


        controller = GetComponent<CarController>();
        controller.Initialize(carControllerValues);

        score = GetComponent<Score>();
        score.Initialize(scoreDistanceMultiplyer, crashScorePenalty);

        scoreShift = 0;

        rng = new System.Random(rngSeed + index);

        Vector3 colorValues = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized;
        Color carColor = new Color(colorValues.x, colorValues.y, colorValues.z);
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            meshRenderer.material.color = carColor;
    }
}