using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public int index;
    [SerializeField] float distance; // sträckan från det föregående checkpointet

    /*
     *  Ger spel objectet värden för när den skapas med kod
     * 
     *  parametrar:
     *      width: bredden på banan
     *      distance: sträckan från det föregående checkpointet
     *      index: index
     */
    public void Initialize(float width, float distance, int index)
    {
        this.index = index;
        gameObject.name = "Checkpoint: " + index.ToString();
        this.distance = distance;
        transform.localScale = new Vector3(1 / 2, 1, width);
    }

    /*  Activeras när en collider kolliderar med objectet
     *  parameter:
     *      other: collidern på objectet som kolliderade med objectet
    */
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
            other.GetComponent<Score>().PassedCheckpoint(index);
    }

    public float GetDistance()
    {
        return distance;
    }
}
