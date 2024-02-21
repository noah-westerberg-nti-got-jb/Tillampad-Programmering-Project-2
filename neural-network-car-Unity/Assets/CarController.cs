using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    [SerializeField] bool control = false;

    [SerializeField] float acceleration = .4f, maxSpeed = 20, rotationSpeed = 180, deceleration = .05f, maxRotationRadius = 10, velocity = 0;

    private float currentAcceleration = 0;

    [SerializeField] Gradient rayColorGradient;

    [SerializeField] Transform front;
    [SerializeField] float viewAngle = 70;
    [SerializeField] float viewDistance = 20;
    [SerializeField] int vissionLines = 5;

    public void Initialize(CarControllerArgs args)
    {
        acceleration = args.acceleration;
        maxSpeed = args.maxSpeed;
        rotationSpeed = args.rotationSpeed;
        deceleration = args.deceleration;
        maxRotationRadius = args.maxRotationRadius;
        viewAngle = args.viewAngle;
        viewDistance = args.viewDistance;
        vissionLines = args.vissionLines;
    }

    void Start()
    {
        Vector3 colorValues = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        Color carColor = new Color(colorValues.x, colorValues.y, colorValues.z);

        foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            meshRenderer.material.color = carColor;
    }

    void Update()
    {
        GetDistances(vissionLines, -viewAngle, viewAngle, viewDistance);

        currentAcceleration = 0;

        if (control)
        {
            if (Input.GetKey(KeyCode.W))
                Accelerate();
            else if (Input.GetKey(KeyCode.S))
                Break();
            if (Input.GetKey(KeyCode.A))
                Turn(-1);
            else if (Input.GetKey(KeyCode.D))
                Turn(1);

            if (Input.GetKeyDown(KeyCode.Space))
                velocity = 0;
        }

        if (velocity >= maxSpeed)
        {
            velocity = maxSpeed;
            currentAcceleration = 0;
        }
        // s = v * t + a * t^2 / 2
        transform.Translate(transform.forward * (velocity * Time.deltaTime + (currentAcceleration * Mathf.Pow(Time.deltaTime, 2) / 2)), 0);
        velocity += (currentAcceleration - deceleration * velocity) * Time.deltaTime;
        if (velocity < 0.01)
            velocity = 0;
    }

    public void Accelerate()
    {
        currentAcceleration = acceleration;
    }

    public void Break()
    {
        currentAcceleration = acceleration / -2;
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

    public float[] GetDistances(int rays, float minAngle, float maxAngle, float viewDistance)
    {
        float[] distances = new float[rays];
        float distanceBetweenRays = (maxAngle - minAngle) / (rays - 1);
        float carAngle = Mathf.Atan2(transform.forward.z, transform.forward.x);

        for (int i = 0; i < rays; i++)
        {
            Vector3 direction = new Vector3(Mathf.Cos(carAngle + (minAngle + (distanceBetweenRays * i)) * Mathf.Deg2Rad), 0, Mathf.Sin(carAngle + (minAngle + (distanceBetweenRays * i)) * Mathf.Deg2Rad)).normalized;

            RaycastHit hit;
            if (Physics.Raycast(front.position, direction, out hit, viewDistance, LayerMask.GetMask("Wall")))
            {
                distances[i] = (front.position - hit.point).magnitude;
            }
            else
                distances[i] = viewDistance;

            Color rayColor = rayColorGradient.Evaluate((viewDistance - distances[i]) / viewDistance);
            Debug.DrawLine(front.position, front.position + direction * distances[i], rayColor);
        }
            return distances;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Wall")
                Debug.Log("Crashed");
        }
    }
