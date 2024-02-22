using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private float timeScale = 1;

    [SerializeField] GameObject car;

    GenerateTrack trackGenerator;

    [SerializeField] float trackLength, trackIncrementSize, trackWidth, trackCheckpointDensity, turnRange, minTurnDistance, maxTurnDistance;
    [SerializeField] int turns;

    [SerializeField] Transform startTransform;
    [SerializeField] int populationSize;
    [SerializeField] float elitCutOf, childToMutationRatio;
    [SerializeField] int[] networkSize;
    [SerializeField] GameObject carObject;
    [SerializeField] CarControllerArgs carControllerValues;
    [SerializeField] int viewLines;
    [SerializeField] float viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty;
    [SerializeField] int rngSeed;

    Learning learning;

    private void Start()
    {
        trackGenerator = GetComponent<GenerateTrack>();
        learning = GetComponent<Learning>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            timeScale = timeScale * 2;
            SetTime(timeScale);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            timeScale = timeScale / 2;
            SetTime(timeScale);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            learning.Inizialize(startTransform, populationSize, elitCutOf, childToMutationRatio, networkSize, car, carControllerValues, viewLines, viewAngle, maxViewDistance, scoreDistanceMultiplyer, crashPenalty, rngSeed);
        }

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    SpawnCar(new Vector3(0, 0, 0));

        //}
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    foreach(GameObject car in cars)
        //        Destroy(car);
        //    foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        //        Destroy(checkpoint);
        //    foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
        //        Destroy(wall);

        //   trackGenerator.Generate(trackLength, trackIncrementSize, trackWidth, turnRange, 0, minTurnDistance, maxTurnDistance, trackCheckpointDensity);
        //}
    }

    void SetTime(float scale)
    {
        Time.timeScale = scale;
        Debug.Log("Time Set to: " + scale);
    }

    //void SpawnCar(Vector3 position)
    //{ 
    //    cars.Add(Instantiate(car, position, Quaternion.identity));
    //    cars[cars.Count - 1].name = "Car: " + (cars.Count - 1).ToString();
    //    // cars[cars.Count - 1].GetComponent<CarController>().index = cars.Count - 1;
    //    carCount = cars.Count;
    //}
}
