using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTrack : MonoBehaviour
{
    [SerializeField] GameObject checkpointObject;

    public void Generate(float length, int turns, float trackWidth)
    {
        float roadAngle = 0, angleToGetBack = Mathf.PI * 2; //radians
        float distanceBetweenTurns = (length - (trackWidth * Mathf.PI)) / turns;
        float distanceBuilt = 0;
        Vector3 checkpointPosition = new Vector3(0, 1, 0);
        createCheckpoint(checkpointPosition, 0, 0, 0, trackWidth);

        for (int i = 0; i < turns; i++)
        {
            Vector3 angleVector = new Vector3(Mathf.Cos(roadAngle), 0, Mathf.Sin(roadAngle));
            Debug.Log(i + " angle: " + roadAngle + " vector: " + angleVector);
            checkpointPosition += distanceBetweenTurns * angleVector;
            createCheckpoint(checkpointPosition, 0, i * 2 + 1, distanceBetweenTurns, trackWidth);
            distanceBuilt += distanceBetweenTurns;

            //float angleChange = Random.value * Mathf.PI;
            //angleToGetBack -= angleChange;
            //if (i == turns - 1)
            //    roadAngle = angleToGetBack;
            //else
            //    roadAngle += angleChange;
            roadAngle += (Mathf.PI * 2) / turns;

            angleVector = new Vector3(Mathf.Cos(roadAngle), 0, Mathf.Sin(roadAngle));
            float turnDistance = roadAngle * (trackWidth / 2);
            checkpointPosition += turnDistance * angleVector;
            //createCheckpoint(checkpointPosition, roadAngle * Mathf.Rad2Deg, i * 2 + 2, turnDistance, trackWidth);
            distanceBuilt += turnDistance;
        }

        createCheckpoint(new Vector3(0, 1, 0), 0, turns * 2 + 1, length - distanceBuilt, trackWidth);
    }

    void createCheckpoint(Vector3 position, float rotation , int index, float distance, float trackWidth)
    {
        Instantiate(checkpointObject, position, 
        Quaternion.Euler(0, rotation, 0))
        .AddComponent<Checkpoint>().initialize(index, distance, trackWidth);
    }
}
