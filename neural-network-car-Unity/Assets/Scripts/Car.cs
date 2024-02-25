using System;
using System.Linq;
using UnityEngine;

[Serializable]
class Car : MonoBehaviour
{
    public NeuralNetwork network;
    CarController controller;
    Score score;
    int index;

    [SerializeField] float currentScore; // atribut för att se värdet i unity inspectorn

    public bool crashed = false;

    // sparar datan så det kan serializeras vid kopiering
    [SerializeField, HideInInspector] int viewLines;
    [SerializeField, HideInInspector] float viewAngle, viewDistance;

    // sparar datan så det kan serializeras vid copiering
    [SerializeField, HideInInspector] CarControllerArgs carControllerValues;
    [SerializeField, HideInInspector] float scoreDistanceMultiplyer, crashScorePenalty;
    // scoreDistanceMultiplyer: multiplicerar poängen get från distance så att, att köra långt lite långsammare väger mer än att köra en kort distance snabbt

    public void Initialize(int index, int[] networkSize, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashScorePenalty, int rngSeed)
    {
        this.index = index;

        controller = GetComponent<CarController>();
        controller.Initialize(carControllerValues);
        this.carControllerValues = carControllerValues;
        this.viewLines = viewLines;
        this.viewAngle = viewAngle;
        viewDistance = maxViewDistance;
        
        network = new NeuralNetwork(networkSize, new System.Random(rngSeed + index));

        score = GetComponent<Score>();
        score.Initialize(scoreDistanceMultiplyer, crashScorePenalty);
        this.scoreDistanceMultiplyer = scoreDistanceMultiplyer;
        this.crashScorePenalty = crashScorePenalty;
    }

    public void Crash()
    {
        crashed = true;
        score.Crash();
        controller.velocity = 0;
    }

    public float GetScore()
    {
        return score.GetScore();
    }

    [SerializeField] string currentlyDoing; // atribut för att se värdet i unity inspectorn

    // tar ett värde som förhåller sig till ett omfång och omvandlar det till ett annat omfång. tagit från arduino map functionen
    double ChangeRange(double valueIn, double inMin, double inMax, double outMin, double outMax)
    {
        return (valueIn - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

    public void Drive()
    {
        currentScore = score.GetScore();

        double[] distances = controller.GetDistances(viewLines, -viewAngle, viewAngle, viewDistance);
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = ChangeRange(distances[i], 0, viewDistance, -1, 1);
        }
        // skapar en input array med syn-distancerna och med bilens hastighet
        double[] input = distances.Concat(new double[] { ChangeRange(controller.velocity, 0, carControllerValues.maxSpeed, -1, 1) }).ToArray();
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
    }

    /*
     *  skapar en copia av componenten på ett nytt object
     *  
     *  parameter:
     *      newCarObject: objectet som componenten ska kopieras till
     *      
     *  https://www.wwt.com/article/how-to-clone-objects-in-dotnet-core : option 1
     */
    public Car Copy(GameObject newCarObject)
    {
        var serialized = JsonUtility.ToJson(this);
        JsonUtility.FromJsonOverwrite(serialized, newCarObject.GetComponent<Car>());
        return newCarObject.GetComponent<Car>();
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

        // anger slumpad färg till bilen
        Vector3 colorValues = new Vector3(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)).normalized;
        Color carColor = new Color(colorValues.x, colorValues.y, colorValues.z);
        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            meshRenderer.material.color = carColor;
    }
}