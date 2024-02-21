using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    GameObject[] checkpoints;
    int currentCheckpointIndex = 0;
    Checkpoint currentCheckpoint;

    float distancePassed, distanceFromPassedCheckpoints, distancePassedToNextCheckpoint;
    float timeSinceStart;

    bool crashed = false;

    float distanceScoreMultiplyer = 1, crashPenalty = 0;

    public void Initialize(float scoreFromDistanceMultiplyer, float crashScorePenalty)
    {
        distanceScoreMultiplyer = scoreFromDistanceMultiplyer;
        crashPenalty = crashScorePenalty;
    }

    public void RetartScore()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        checkpoints = SortCheckpoints(checkpoints);

        currentCheckpoint = checkpoints[0].GetComponent<Checkpoint>();

        timeSinceStart = 0;

        crashed = false;

        distancePassed = 0;
        distanceFromPassedCheckpoints = 0;
    }

    private void Update()
    {
        if (!crashed)
        {
            timeSinceStart += Time.deltaTime;


            // Debug.Log("i: " + currentCheckpointIndex + " p :" + currentCheckpoint.GetPosition());

            distancePassedToNextCheckpoint = currentCheckpoint.GetDistance() - (transform.position - currentCheckpoint.GetPosition()).magnitude;
            distancePassed = distanceFromPassedCheckpoints + distancePassedToNextCheckpoint;


            // Debug.Log("D: " + distancePassed[0] + " T: " + timeSinceStart + " S: " + Score(0));

            // Debug.DrawLine(currentCheckpoint.GetPosition(), currentCheckpoint.GetPosition() + Vector3.up * 10, Color.green);
        }
    }

    public void PassedCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex != currentCheckpointIndex || crashed) return;

        distanceFromPassedCheckpoints += currentCheckpoint.GetDistance();

        currentCheckpointIndex++;
        currentCheckpoint = checkpoints[currentCheckpointIndex].GetComponent<Checkpoint>();
    }

    public float GetScore()
    {
        if (crashed)
            return (distancePassed * distanceScoreMultiplyer / timeSinceStart) + crashPenalty;
        return distancePassed * distanceScoreMultiplyer / timeSinceStart;
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

    void Stop()
    {
        crashed = true;
    }
}
