using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{

    [SerializeField] bool control = false;

    [SerializeField] float acceleration = .4f, maxSpeed = 20, rotationSpeed = 180, slowDown = .05f, maxRotationRadius = 10;

    private float velocity = 0;

    void Start()
    {
        Vector3 colorValues = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        Color carColor = new Color(colorValues.x, colorValues.y, colorValues.z);

        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            meshRenderer.material.color = carColor;
    }

    void Update() 
    {
        if (control)
        {
            if (Input.GetKey(KeyCode.W))
                Accelerate();
            if (Input.GetKey(KeyCode.A))
                Turn(-1);
            else if (Input.GetKey(KeyCode.D))
                Turn(1);

            if (Input.GetKeyDown(KeyCode.Space))
                velocity = 0;
        }

        transform.Translate(transform.forward * velocity * Time.deltaTime, 0);

        if (velocity > 0.01)
            velocity -= slowDown * velocity * Time.deltaTime + acceleration / 10;
        else
            velocity = 0;
    }

    public void Accelerate()
    {
        velocity += acceleration;
        if (velocity > maxSpeed)
            velocity = maxSpeed;
    }

    public void Turn(int direction)
    {
        float rotation;
        if (velocity / (Mathf.Deg2Rad * rotationSpeed) > maxRotationRadius) // Cirkelsektor
            rotation = (velocity / maxRotationRadius) * Mathf.Rad2Deg * direction * Time.deltaTime;
        else
            rotation = direction * rotationSpeed * Time.deltaTime;

        transform.Rotate(new Vector3(0, rotation), Space.Self);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "wall")
            Debug.Log("Crashed");
    }
}
