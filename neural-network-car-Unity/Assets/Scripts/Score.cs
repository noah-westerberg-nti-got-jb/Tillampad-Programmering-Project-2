using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Score : MonoBehaviour
{
    [SerializeField, HideInInspector] GameObject[] checkpoints;
    int currentCheckpointIndex = 0;
    Checkpoint currentCheckpoint;

    float distancePassed, distanceFromPassedCheckpoints, distancePassedToNextCheckpoint;
    float timeSinceStart;

    bool crashed = true;

    bool finished = false;

    [SerializeField, HideInInspector] float distanceScoreMultiplyer, crashPenalty;

    public void Initialize(float scoreFromDistanceMultiplyer, float crashScorePenalty)
    {
        distanceScoreMultiplyer = scoreFromDistanceMultiplyer;
        crashPenalty = crashScorePenalty;

        StartScore();
    }

    public void StartScore()
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
        if (crashed) return;
        

        timeSinceStart += Time.deltaTime;

        // Debug.Log("i: " + currentCheckpointIndex + " p :" + currentCheckpoint.GetPosition());

        distancePassedToNextCheckpoint = currentCheckpoint.GetDistance() - (transform.position - currentCheckpoint.GetPosition()).magnitude;
        distancePassed = distanceFromPassedCheckpoints + distancePassedToNextCheckpoint;


        // Debug.Log("D: " + distancePassed[0] + " T: " + timeSinceStart + " S: " + Score(0));

        // Debug.DrawLine(currentCheckpoint.GetPosition(), currentCheckpoint.GetPosition() + Vector3.up * 10, Color.green);
    }

    public void PassedCheckpoint(int checkpointIndex)
    {
        if (checkpointIndex != currentCheckpointIndex || crashed) return;

        if (checkpointIndex == checkpoints.Length - 1)
        {
            finished = true;
            crashed = true;
            return;
        }

        distanceFromPassedCheckpoints += currentCheckpoint.GetDistance();

        currentCheckpointIndex++;
        currentCheckpoint = checkpoints[currentCheckpointIndex].GetComponent<Checkpoint>();
    }

    public float GetScore()
    {
        float finishBonus = 0;
        if (finished)
            finishBonus = 10000000000;
        else if (crashed)
            return (distancePassed * distanceScoreMultiplyer / timeSinceStart) - (crashPenalty / timeSinceStart);
        return (distancePassed * distanceScoreMultiplyer / timeSinceStart) + finishBonus;
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

    public void Crash()
    {
        crashed = true;
    }
}
