using UnityEngine;

class Car : MonoBehaviour
{
    NeuralNetwork network;
    CarController controller;
    Score score;
    public int index;

    bool crashed = false;

    public Car(int index, int[] networkSize, CarControllerArgs carControllerValues, float scoreDistanceMultiplyer, float crashScorePenalty)
    {
        this.index = index;

        controller = gameObject.AddComponent<CarController>();
        controller.Initialize(carControllerValues);

        network = new NeuralNetwork(networkSize, new System.Random());

        score = gameObject.AddComponent<Score>();
        score.Initialize(scoreDistanceMultiplyer, crashScorePenalty);
        
    }

    public Car(int index, int[] networkSize, CarControllerArgs carControllerValues, float scoreDistanceMultiplyer, float crashScorePenalty, int rngSeed)
    {
        this.index = index;

        controller = gameObject.AddComponent<CarController>();
        controller.Initialize(carControllerValues);

        System.Random rng = new System.Random(rngSeed);
        network = new NeuralNetwork(networkSize, rng);

        score = gameObject.AddComponent<Score>();
        score.Initialize(scoreDistanceMultiplyer, crashScorePenalty);
    }

    void Crash()
    {
        crashed = true;
    }
}