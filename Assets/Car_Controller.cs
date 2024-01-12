using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Controller : MonoBehaviour
{

    [SerializeField] bool control = false;

    private float velocity = 0;

    [SerializeField] float acceleration = .4f;

    [SerializeField] float maxSpeed = 20;

    [SerializeField] float rotationSpeed = 50;

    [SerializeField] float slowDown = .05f;

    void Update() 
    {
        if (control)
        {
            if (Input.GetKey(KeyCode.W))
                accelerate();
            if (Input.GetKey(KeyCode.A))
                turn(-1);
            else if (Input.GetKey(KeyCode.D))
                turn(1);
        }

        transform.Translate(transform.forward * velocity * Time.deltaTime, Space.Self);

        if (velocity > 0.01)
            velocity -= slowDown * velocity * Time.deltaTime + acceleration / 10;
        else
            velocity = 0;
    }

    void accelerate()
    {
        velocity += acceleration;
        if (velocity > maxSpeed)
            velocity = maxSpeed;
    }

    void turn(int direction)
    {   
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime), Space.Self);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "wall")
            Debug.Log("Crashed");
    }
}
