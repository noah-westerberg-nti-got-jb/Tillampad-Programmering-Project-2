using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    GameObject[] checkpoints;
    int currentCheckpointIndex = 0;
    Checkpoint currentCheckpoint;

    float distancePassed = 0, distanceFromPassedCheckpoints, timeSinceStart = 0, distancePassedToNextCheckpoint;

    private void OnEnable()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        currentCheckpoint = checkpoints[currentCheckpointIndex].GetComponent<Checkpoint>();
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        distancePassedToNextCheckpoint = currentCheckpoint.GetDistance() - (transform.position - currentCheckpoint.GetComponent<Transform>().position).magnitude;
        distancePassed = distanceFromPassedCheckpoints + distancePassedToNextCheckpoint;
    }

    public void PassedCheckpoint(int index)
    {
        if (index != currentCheckpointIndex) return;

        distanceFromPassedCheckpoints += currentCheckpoint.GetDistance();

        currentCheckpointIndex++;
    } 

    public float Score()
    {
        return distancePassed / timeSinceStart;
    }
}
