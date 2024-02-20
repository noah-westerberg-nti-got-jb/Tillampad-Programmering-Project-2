using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    GameObject[] checkpoints, cars;
    int[] currentCheckpointIndexs;
    Checkpoint[] currentCheckpoints;

    float[] distancePassed, distanceFromPassedCheckpoints, distancePassedToNextCheckpoint;
    float timeSinceStart;

    void Awake()
    {
        enabled = false;
    }


    private void OnEnable()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpoints = SortCheckpoints(checkpoints);

        cars = GameObject.FindGameObjectsWithTag("Car");
        cars = SortCars(cars);
        distancePassed = new float[cars.Length];
        distanceFromPassedCheckpoints = new float[cars.Length];
        distancePassedToNextCheckpoint = new float[cars.Length];

        currentCheckpointIndexs = new int[cars.Length];
        currentCheckpoints = new Checkpoint[cars.Length];
        for (int i = 0; i < cars.Length; i++)
        {
            currentCheckpointIndexs[i] = 0;
            currentCheckpoints[i] = checkpoints[0].GetComponent<Checkpoint>();
        }
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        for (int i = 0; i < cars.Length; i++)
        {
            // Debug.Log("i: " + currentCheckpointIndex + " p :" + currentCheckpoint.GetPosition());

            distancePassedToNextCheckpoint[i] = currentCheckpoints[i].GetDistance() - (cars[i].transform.position - currentCheckpoints[i].GetPosition()).magnitude;
            distancePassed[i] = distanceFromPassedCheckpoints[i] + distancePassedToNextCheckpoint[i];
        }

        // Debug.Log("D: " + distancePassed[0] + " T: " + timeSinceStart + " S: " + Score(0));

        Debug.DrawLine(currentCheckpoints[0].GetPosition(), currentCheckpoints[0].GetPosition() + Vector3.up * 10, Color.green);
    }

    public void PassedCheckpoint(int checkpointIndex, int carIndex)
    {
        if (checkpointIndex != currentCheckpointIndexs[carIndex]) return;

        distanceFromPassedCheckpoints[carIndex] += currentCheckpoints[carIndex].GetDistance();

        currentCheckpointIndexs[carIndex]++;
        currentCheckpoints[carIndex] = checkpoints[currentCheckpointIndexs[carIndex]].GetComponent<Checkpoint>();
    }

    public float Score(int carIndex)
    {
        return distancePassed[carIndex] / timeSinceStart;
    }

    GameObject[] SortCheckpoints(GameObject[] checkpoints)
    {
        GameObject[] sorted = new GameObject[checkpoints.Length];

        foreach (GameObject checkpoint in checkpoints)
        {
            int checkpointIndex = checkpoint.GetComponent<Checkpoint>().index;
            sorted[checkpointIndex] = checkpoint;
        }

        return sorted;
    }

    GameObject[] SortCars(GameObject[] cars)
    {
        GameObject[] sorted = new GameObject[cars.Length];

        foreach (GameObject car in cars)
        {
            int carIndex = car.GetComponent<CarController>().index;
            sorted[carIndex] = car;
        }

        return sorted;
    }
}
