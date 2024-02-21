using System.Collections;

using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;

public class Learning : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}


class GeneticLearning
{
    float elitCutOf, childToMutationRatio;

    public GeneticLearning(int populationSize, float elitCutOf, float childToMutationRatio, int[] networkSize, CarControllerArgs carControllerValues)
    {

        this.elitCutOf = elitCutOf;
        this.childToMutationRatio = childToMutationRatio;

        Car[] population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new Car(i, networkSize, carControllerValues);
        }
    }
    public GeneticLearning(int populationSize, float elitCutOf, float childToMutationRatio, int[] networkSize, CarControllerArgs carControllerValues, int rngSeed)
    {

        this.elitCutOf = elitCutOf;
        this.childToMutationRatio = childToMutationRatio;

        Car[] population = new Car[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            population[i] = new Car(i, networkSize, carControllerValues, rngSeed);
        }
    }
    
    void Evolve()
    {

    }
}