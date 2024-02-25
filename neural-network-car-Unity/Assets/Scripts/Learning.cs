using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Learning : MonoBehaviour
{
    GeneticLearning population;
    bool inizialized = false;

    public void Inizialize(Transform spawnTransform, int populationSize, float elitCutOf, float childToMutationRatio, int[] networkSize, GameObject carObject, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashPenalty, int rngSeed)
    {
        inizialized = true;
        population = new GeneticLearning(spawnTransform, populationSize, elitCutOf, childToMutationRatio, networkSize, carObject, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed);
    }

    void Update()
    {
        if (!inizialized) return;

        bool stillGoing = false; // kollar om det finns bilar som inte har krachat

        // går igenom alla bilar
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

    Transform spawnTransform;
    GameObject carObject;

    // Constructor
    public GeneticLearning(Transform spawnTransform, int populationSize, float eliteCutOf, float childToMutationRatio, int[] networkSize, GameObject carObject, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashPenalty, int rngSeed)
    {

        this.eliteCutOf = eliteCutOf;
        this.childToMutationRatio = childToMutationRatio;

        this.populationSize = populationSize;
        population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = Object.Instantiate(carObject, spawnTransform).GetComponent<Car>(); // skapar bil-objectet
            population[i].Initialize(i, networkSize, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed + i * i + 1);
        }

        rng = new System.Random(rngSeed * rngSeed);

        this.spawnTransform = spawnTransform;
        this.carObject = carObject;
    }

    // struct som inehåller en bil class och bilens poäng
    public struct CarScoreStruct
    {
        public Car car;
        public float score;
    }

    /*
     * bubble sort för bilarnas poäng
     * 
     * tidskomplexitet blir inte ett problem för att det kan inte finnas så många bilar
     * 
     * https://www.w3resource.com/csharp-exercises/searching-and-sorting-algorithm/searching-and-sorting-algorithm-exercise-3.php
     */
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
        CarScoreStruct[] scoreCar = new CarScoreStruct[populationSize]; // array som innehåller bilar och deras poäng. kunde inte komma på något bättre namn
        // ange värden till alla element
        for (int i = 0; i < populationSize; i++)
        {
            scoreCar[i].car = population[i];
            scoreCar[i].score = population[i].GetScore();
        }
        // sortera
        SortCarsByScore(scoreCar);

        Car[] sortedCars = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            // elementen läggs in backlänges så att den 0:te bilen har högst poäng och den sista bilen har lägst
            sortedCars[i] = scoreCar[populationSize - 1 - i].car; // 
        }

        return sortedCars;
    }

    public void Evolve()
    {
        Car[] scoredCars = GetScores();
        Car[] newPopulation = new Car[populationSize];
        // går igenom och anger värden till den nya populationen
        for (int i = 0; i < populationSize; i++)
        {
            // går vidare oändrade
            if (i < populationSize * eliteCutOf)
            {
                newPopulation[i] = scoredCars[i].Copy(Object.Instantiate(carObject, spawnTransform)); // skapar en ny copia
                newPopulation[i].SetIndex(i);
                continue;
            }
            // skapar "barn"
            if (i < (populationSize * eliteCutOf) + ((populationSize - (populationSize * eliteCutOf)) * childToMutationRatio))
            {
                // anger "föräldrar"
                Car[] parrents = new Car[] { scoredCars[RandomFromPopulation()], scoredCars[RandomFromPopulation()] };
                Car child = parrents[0].Copy(Object.Instantiate(carObject, spawnTransform)); // gör "barnet" till en kopia av en av "föräldrarna"
                NetworkLayer[] childLayers = new NetworkLayer[child.network.GetLayers().Length];
                // går igenom alla lager i nätverket
                for (int j = 0; j < childLayers.Length; j++)
                {
                    // tar fram värden från "föräldrarna"
                    double[] biasesParent1 = parrents[0].network.GetLayers()[j].GetBiases(),
                             biasesParent2 = parrents[1].network.GetLayers()[j].GetBiases();
                    double[,] weightsParent1 = parrents[0].network.GetLayers()[j].GetWeights(),
                              weightsParent2 = parrents[1].network.GetLayers()[j].GetWeights();

                    double[] newBiases = biasesParent1;
                    // skapar nätverks lagret
                    childLayers[j] = new NetworkLayer(weightsParent1.GetLength(1), biasesParent1.Length);
                    //går igenom alla noder
                    for (int k = 0; k < weightsParent1.GetLength(0); k++)
                    {
                        if (rng.NextDouble() >= 0.5) // slumpar mellan att ta värdet från "förälder" 1 eller 2
                            newBiases[k] = biasesParent2[k];

                        double[] newWeights = new double[weightsParent1.GetLength(1)];
                        // går igenom alla kopplingar till noden
                        for (int l = 0; l < newWeights.Length; l++)
                        {
                            if (rng.NextDouble() < 0.5) // slumpar mellan att ta värdet från "förälder" 1 eller 2
                                newWeights[l] = weightsParent1[k, l];
                            else
                                newWeights[l] = weightsParent2[k, l];
                        }
                        // anger de nya vikterna till noden
                        childLayers[j].SetWeights(k, newWeights);
                    }
                    // anger de nya aktiveringstalen till noderna i lagret
                    childLayers[j].SetBiases(newBiases);
                }
                // anger lagerna till nätverket
                child.network.SetLayers(childLayers);
                newPopulation[i] = child;
                newPopulation[i].SetIndex(i);
                continue;
            }
            // skapar muterade bilar/nätverk 
            int random = RandomFromPopulation();
            Car mutated = scoredCars[random].Copy(Object.Instantiate(carObject, spawnTransform)); // skapar en kopia av en slumpad bil
            // går igenom alla lagerna
            foreach (NetworkLayer layer in mutated.network.GetLayers())
            {
                double[] biasShift = new double[layer.outputNodesNum];
                // går igenom noderna
                for (int j = 0; j < layer.outputNodesNum; j++)
                {
                    biasShift[j] = (rng.NextDouble() * 2) - 1; // anger ett slumpat tal mellan -1 och 1

                    double[] weightShift = new double[layer.inputNodesNum];
                    // går igenom kopplingaran till noderna
                    for (int k = 0; k < layer.inputNodesNum; k++)
                    {
                        weightShift[k] = (rng.NextDouble() * 2) - 1;// anger ett slumpat tal mellan -1 och 1
                    }
                    layer.ShiftWeights(j, weightShift);
                }
                layer.ShiftBiases(biasShift);
            }
            newPopulation[i] = mutated;
            newPopulation[i].SetIndex(i);
        }

        // tar bort den gamla populationen
        DestroyCars();

        population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = newPopulation[i];
            population[i].crashed = false;
        }
    }

    // en slumpad bil från populationen med högre sannolikhet att få högrepresterande bilar än lägre
    int RandomFromPopulation()
    {
        return (int)((1 - System.Math.Pow(rng.NextDouble(), 2)) * populationSize);
    }

    // tar bort alla bilar i populationen
    void DestroyCars()
    {
        foreach (Car car in population)
        {
            Object.Destroy(car.gameObject);
        }
    }
}