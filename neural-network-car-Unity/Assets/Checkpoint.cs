using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Checkpoint(int index, float distance)
    {
        this.index = index;
    }
    public bool isPassed;
    private int index, distance;

    private void OnTriggerEnter(Collider other)
    {
        if (isPassed) return;
        
        if (other.tag == "Car")
            other.GetComponent<ScoreCalculator>().PassedCheckpoint(index);
    }

    public float GetDistance()
    {
        return distance;
    }
}
