using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    GameObject[] checkpoints, cars;
    int currentCheckpointIndex = 0;
    Checkpoint currentCheckpoint;

    float[] distancePassed, distanceFromPassedCheckpoints, distancePassedToNextCheckpoint;
    float timeSinceStart;

    private void OnEnable()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        currentCheckpoint = checkpoints[currentCheckpointIndex].GetComponent<Checkpoint>();

        cars = GameObject.FindGameObjectsWithTag("Car");
        distancePassed = new float[cars.Length];
        distanceFromPassedCheckpoints = new float[cars.Length];
        distancePassedToNextCheckpoint = new float[cars.Length];
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        for (int i = 0; i < cars.Length; i++)
        { 
            distancePassedToNextCheckpoint[i] = currentCheckpoint.GetDistance() - (transform.position - currentCheckpoint.GetComponent<Transform>().position).magnitude;
            distancePassed[i] = distanceFromPassedCheckpoints[i] + distancePassedToNextCheckpoint[i];
        }
    }

    public void PassedCheckpoint(int checkpointIndex, int carIndex)
    {
        if (checkpointIndex != currentCheckpointIndex) return;

        distanceFromPassedCheckpoints[carIndex] += currentCheckpoint.GetDistance();

        currentCheckpointIndex++;
    }

    public float Score(int carIndex)
    {
        return distancePassed[carIndex] / timeSinceStart;
    }
}
