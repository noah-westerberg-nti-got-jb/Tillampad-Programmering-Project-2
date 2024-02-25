

//  Blev inte klar med track-genereringen så koden här används inte och jag kommenterar inte koden




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTrack : MonoBehaviour
{
    [SerializeField] GameObject checkpoint, wall;

    public void Generate(float length, float incrementSize, float trackWidth, float turnRangeDeg, float turnRangeSpread, float minTurnRate, float maxTurnRate, float checkpointDensity)
    {
        float[] turnRange = { 
            -turnRangeDeg * Mathf.Deg2Rad, 
            turnRangeDeg * Mathf.Deg2Rad 
        };

        float currentAngle = 0;
        float desiredAngle = 0;

        Vector3 currentPosition = Vector3.zero;
        Vector3 desiredPosition = Vector3.zero;

        float distanceBuilt = 0;

        int incrementsLeft = 0;

        for (int i = 0; i < length / incrementSize; i++)
        { 
            if (incrementsLeft == 0)
            {
                Vector3 vectorToEnd = Vector3.zero - currentPosition;
                float newAngleCenter = (currentAngle + (Mathf.Atan2(vectorToEnd.x, vectorToEnd.z) * (distanceBuilt / length) * (vectorToEnd.magnitude / (length - distanceBuilt)))) / 2;
                float[] newAngleRange = { 
                    newAngleCenter + turnRange[0],
                    newAngleCenter + turnRange[1] 
                };
                desiredAngle = newAngleRange[0] + (2 * newAngleRange[1] * Random.value);
                currentAngle = desiredAngle;

                float minLength = (maxTurnRate * Mathf.Deg2Rad) / desiredAngle;
                float maxLength = (minTurnRate * Mathf.Deg2Rad) / desiredAngle;
                float turnLength = (maxLength - minLength) * Random.value;

                desiredPosition = currentPosition + new Vector3(Mathf.Cos(desiredAngle), 0, Mathf.Sin(desiredAngle)) * turnLength;

                Debug.DrawLine(currentPosition, desiredPosition, Color.red, 10);
            }
            incrementsLeft = (int)((desiredPosition - currentPosition).magnitude / incrementSize);
            //Debug.Log(incrementsLeft + " " + 1 / incrementsLeft);
            //currentAngle = Mathf.Lerp(currentAngle, desiredAngle, 1 / (float)incrementsLeft);

            if ((int)(i % (1 / checkpointDensity)) == 0)
                CreateSection(currentPosition, currentAngle, trackWidth, incrementSize, (int)(i * checkpointDensity));
            else
                CreateSection(currentPosition, currentAngle, trackWidth, incrementSize);

            currentPosition += new Vector3(Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle)) * incrementSize;

            distanceBuilt += incrementSize;

            Debug.Log("Pos: " + currentPosition + " Angle: " + currentAngle + " dPos: " + desiredPosition + " dAngle: " + desiredAngle);
        }
    }

    void CreateCheckpoint(Vector3 position, float rotation , int index, float distance, float trackWidth)
    {
        Debug.Log("checkpoint: " + index + " rotation: " + rotation * Mathf.Rad2Deg);
        Instantiate(checkpoint, position,
        Quaternion.Euler(0, rotation * Mathf.Rad2Deg, 0))
        .AddComponent<Checkpoint>().Initialize(trackWidth, distance, index);
    }
    void CreateSection(Vector3 position, float rotation, float trackWidth, float sectionLength, int checkpointIndex)
    {
        CreateCheckpoint(position, rotation, checkpointIndex, sectionLength, trackWidth);

        for (int i = -1; i < 2; i += 2)
        {
            Vector3 horizontalOffset = new Vector3(Mathf.Cos(rotation + Mathf.PI / 2), 0, Mathf.Sin(rotation + Mathf.PI / 2)) * trackWidth * i * 0.5f;
            Vector3 verticalOffset = new Vector3(Mathf.Cos(rotation), 0, Mathf.Sin(rotation)) * sectionLength * 0.5f;

            Instantiate(wall,
            position + horizontalOffset + verticalOffset,
            Quaternion.Euler(0, rotation * Mathf.Rad2Deg, 0)).AddComponent<Wall>().Initialize(sectionLength);
        }
    }
    void CreateSection(Vector3 position, float rotation, float trackWidth, float sectionLength)
    {
        for (int i = -1; i < 2; i += 2)
        {
            Vector3 horizontalOffset = new Vector3(Mathf.Cos(rotation + Mathf.PI / 2), 0, Mathf.Sin(rotation + Mathf.PI / 2)) * trackWidth * i * 0.5f;
            Vector3 verticalOffset = new Vector3(Mathf.Cos(rotation), 0, Mathf.Sin(rotation)) * sectionLength * 0.5f;

            Instantiate(wall,
            position + horizontalOffset + verticalOffset,
            Quaternion.Euler(0, rotation, 0)).AddComponent<Wall>().Initialize(sectionLength);
        }
    }
}
