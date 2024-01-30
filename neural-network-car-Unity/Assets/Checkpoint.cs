using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] float distance;
    public void initialize(int index, float distance, float width)
    {
        this.index = index;
        this.distance = distance;
        transform.localScale = new Vector3(1, 1, width);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
            other.GetComponent<ScoreCalculator>().PassedCheckpoint(index, int.Parse(other.name));
    }

    public float GetDistance()
    {
        return distance;
    }
}
