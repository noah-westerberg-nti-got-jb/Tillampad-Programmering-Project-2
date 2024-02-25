using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public int index;
    [SerializeField] float distance;

    public void Initialize(float width, float distance, int index)
    {
        this.index = index;
        gameObject.name = "Checkpoint: " + index.ToString();
        this.distance = distance;
        transform.localScale = new Vector3(1 / 2, 1, width);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
            other.GetComponent<Score>().PassedCheckpoint(index);
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
