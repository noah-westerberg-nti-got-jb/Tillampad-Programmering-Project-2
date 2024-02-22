using System.Collections;

using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class Learning : MonoBehaviour
{
    GeneticLearning population;
    bool inizialized = false;

    public void Inizialize(Transform startTransform, int populationSize, float elitCutOf, float childToMutationRatio, int[] networkSize, GameObject carObject, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashPenalty, int rngSeed)
    {
        inizialized = true;
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

        if (stillGoing) return;




    }
}


class GeneticLearning
{
    float elitCutOf, childToMutationRatio;
    public Car[] population;

    public GeneticLearning(Transform startTransform, int populationSize, float elitCutOf, float childToMutationRatio, int[] networkSize, GameObject carObject, CarControllerArgs carControllerValues, int viewLines, float viewAngle, float maxViewDistance, float scoreDistanceMultiplyer, float crashPenalty , int rngSeed)
    {

        this.elitCutOf = elitCutOf;
        this.childToMutationRatio = childToMutationRatio;

        population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = Object.Instantiate(carObject, startTransform).GetComponent<Car>();
            population[i].Initialize(i, networkSize, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed + i);
        }
    }
    
    void Evolve()
    {

    }

    public void resetTransfomrs(Transform transform)
    {
        foreach (Car car in population)
        {
            car.transform.position = transform.position;
            car.transform.rotation = transform.rotation;
            car.transform.localScale = transform.localScale;
        }
    }
}