using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Learning : MonoBehaviour
{
    GeneticLearning population;
    bool inizialized = false;

    Transform startTransform;

    public void Inizialize(Transform startTransform, int populationSize, float elitCutOf, float childToMutationRatio, int[] networkSize, GameObject carObject, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashPenalty, int rngSeed)
    {
        inizialized = true;
        this.startTransform = startTransform;
        population = new GeneticLearning(startTransform, populationSize, elitCutOf, childToMutationRatio, networkSize, carObject, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed);
    }

    void Update()
    {
        if (!inizialized) return;

        bool stillGoing = false;

        foreach (Car car in population.population)
        {
            if (car.crashed) continue;

            car.Drive();

            stillGoing = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stillGoing = false;
        }

        if (stillGoing) return;


        population.Evolve();
    }
}


class GeneticLearning
{
    float eliteCutOf, childToMutationRatio;
    public Car[] population;
    int populationSize;
    System.Random rng;

    Transform startTransform;
    GameObject carObject;
    CarControllerArgs carControllerValues;
    int viewLines;
    float viewAngle, maxViewDistance;
    float scoreDistanceMultiplyer;
    float crashPenalty;
    int rngSeed;

    public GeneticLearning(Transform startTransform, int populationSize, float eliteCutOf, float childToMutationRatio, int[] networkSize, GameObject carObject, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashPenalty, int rngSeed)
    {

        this.eliteCutOf = eliteCutOf;
        this.childToMutationRatio = childToMutationRatio;

        this.populationSize = populationSize;
        population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = Object.Instantiate(carObject, startTransform).GetComponent<Car>();
            population[i].Initialize(i, networkSize, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed + i * i + 1);
        }

        rng = new System.Random(rngSeed * rngSeed);

        this.startTransform = startTransform;
        this.carObject = carObject;
        this.carControllerValues = carControllerValues;
        this.viewLines = viewLines;
        this.viewAngle = viewAngle;
        this.maxViewDistance = maxViewDistance;
        this.scoreDistanceMultiplyer = scoreDistanceMultiplyer;
        this.crashPenalty = crashPenalty;
        this.rngSeed = rngSeed;
    }

    public struct CarScoreStruct
    {
        public Car car;
        public float score;
    }

    // https://www.w3resource.com/csharp-exercises/searching-and-sorting-algorithm/searching-and-sorting-algorithm-exercise-3.php
    void SortCarsByScore(CarScoreStruct[] values)
    {
        for (int i = 0; i <= values.Length - 2; i++) // Outer loop for passes
        {
            for (int j = 0; j <= values.Length - 2; j++) // Inner loop for comparison and swapping
            {
                if (values[j].score > values[j + 1].score) // Checking if the current element is greater than the next element
                {
                    var temp = values[j + 1]; // Swapping elements if they are in the wrong order
                    values[j + 1] = values[j];
                    values[j] = temp;
                }
            }
        }
    }

    Car[] GetScores()
    {
        /* Dictionary<float, Car> scoreCar = new();
        foreach (Car car in population)
        {
            float carScore = car.GetScore() + car.ScoreShift();
            Debug.Log("index: " + car.index + " base: " + car.GetScore() + " shift: " + car.ScoreShift() + " score: " + carScore);
            scoreCar.Add(carScore, car);
        }

        float[] scores = new float[population.Length];
        for (int i = 0; i < population.Length; i++)
        {
            scores[i] = population[i].GetScore() + population[i].ScoreShift();
        }
        System.Array.Sort(scores); 

        Car[] sortedScoreCar = new Car[population.Length];
        for (int i = 0; i < population.Length; i++)
        {
            sortedScoreCar[i] = scoreCar[scores[population.Length - 1 - i]];
        } */

        CarScoreStruct[] scoreCar = new CarScoreStruct[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            scoreCar[i].car = population[i];
            scoreCar[i].score = population[i].GetScore();
        }
        SortCarsByScore(scoreCar);

        Car[] sortedCars = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            sortedCars[i] = scoreCar[populationSize - 1 - i].car;
        }

        foreach (Car car in sortedCars)
        {
            // Debug.Log(car.name);
        }

        return sortedCars;
    }

    public void Evolve()
    {
        Car[] scoredCars = GetScores();
        Car[] newPopulation = new Car[populationSize];

        for (int i = 0; i < populationSize; i++)
        {
            // Debug.Log("enter loop");

            if (i < populationSize * eliteCutOf)
            {
                // Debug.Log("elite");


                newPopulation[i] = scoredCars[i].Copy(Object.Instantiate(carObject, startTransform));
                newPopulation[i].SetIndex(i);
                newPopulation[i].SetRNG(rng.Next());
                continue;
            }

            if (i < (populationSize * eliteCutOf) + ((populationSize - (populationSize * eliteCutOf)) * childToMutationRatio))
            {
                // Debug.Log("parrents");

                Car[] parrents = new Car[] { scoredCars[RandomFromPopulation()], scoredCars[RandomFromPopulation()] };
                Car child = parrents[0].Copy(Object.Instantiate(carObject, startTransform));
                NetworkLayer[] childLayers = new NetworkLayer[child.network.GetLayers().Length];
                for (int j = 0; j < childLayers.Length; j++)
                {
                    double[] biasesP1 = parrents[0].network.GetLayers()[j].GetBiases(),
                             biasesP2 = parrents[1].network.GetLayers()[j].GetBiases();
                    double[,] weightsP1 = parrents[0].network.GetLayers()[j].GetWeights(),
                              weightsP2 = parrents[1].network.GetLayers()[j].GetWeights();

                    double[] newBiases = biasesP1;

                    childLayers[j] = new NetworkLayer(weightsP1.GetLength(1), biasesP1.Length);

                    for (int k = 0; k < weightsP1.GetLength(0); k++)
                    {
                        if (rng.NextDouble() >= 0.5)
                            newBiases[k] = biasesP2[k];

                        double[] newWeights = new double[weightsP1.GetLength(1)];
                        for (int l = 0; l < newWeights.Length; l++)
                        {
                            if (rng.NextDouble() < 0.5)
                                newWeights[l] = weightsP1[k, l];
                            else
                                newWeights[l] = weightsP2[k, l];
                        }
                        childLayers[j].SetWeights(k, newWeights);
                    }
                    childLayers[j].SetBiases(newBiases);
                }
                child.network.SetLayers(childLayers);
                newPopulation[i] = child;
                newPopulation[i].SetIndex(i); 
                newPopulation[i].SetRNG(rng.Next());
                continue;
            }

            // Debug.Log("mutate");

            int random = RandomFromPopulation();
            Car mutated = scoredCars[random].Copy(Object.Instantiate(carObject, startTransform));
            foreach (NetworkLayer layer in mutated.network.GetLayers())
            {
                double[] biasShift = new double[layer.outputNodesNum];
                for (int j = 0; j < layer.outputNodesNum; j++)
                {
                    biasShift[j] = (rng.NextDouble() * 2) - 1;

                    double[] weightShift = new double[layer.inputNodesNum];
                    for (int k = 0; k < layer.inputNodesNum; k++)
                    {
                        weightShift[k] = (rng.NextDouble() * 2) - 1;
                    }
                    layer.ShiftWeights(j, weightShift);
                }
                layer.ShiftBiases(biasShift);
            }
            newPopulation[i] = mutated;
            newPopulation[i].SetIndex(i);
            newPopulation[i].SetRNG(rng.Next());
        }

        DestroyCars();

        population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = newPopulation[i];
            population[i].crashed = false;
        }
    }

    int RandomFromPopulation()
    {
        return (int)((1 - System.Math.Pow(rng.NextDouble(), 2)) * populationSize);
    }

    public void ResetTransforms(Transform transform)
    {
        foreach (Car car in population)
        {
            car.transform.position = transform.position;
            car.transform.rotation = transform.rotation;
        }
    }

    void DestroyCars()
    {
        foreach (Car car in population)
        {
            Object.Destroy(car.gameObject);
        }
    }
}