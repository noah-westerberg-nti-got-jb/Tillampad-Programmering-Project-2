using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public int index;
    [SerializeField] float distance;

    ScoreCalculator scoreCalculator;

    public void Initialize(float width, float distance, int index)
    {
        this.index = index;
        gameObject.name = "Checkpoint: " + index.ToString();
        this.distance = distance;
        transform.localScale = new Vector3(1 / 2, 1, width);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (scoreCalculator == null)
            scoreCalculator = GameObject.Find("Manager").GetComponent<ScoreCalculator>();

        if (other.tag == "Car")
            scoreCalculator.PassedCheckpoint(index, other.GetComponent<CarController>().index);
    }

    public float GetDistance()
    {
        return distance;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
