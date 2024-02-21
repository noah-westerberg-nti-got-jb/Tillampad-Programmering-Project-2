using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private float time = 1;

    [SerializeField] GameObject car;
    private List<GameObject> cars = new List<GameObject>();

    public int carCount;

    GenerateTrack trackGenerator;

    [SerializeField] float trackLength, trackIncrementSize, trackWidth, trackCheckpointDensity, turnRange, minTurnDistance, maxTurnDistance;
    [SerializeField] int turns;

    private void Start()
    {
        trackGenerator = GetComponent<GenerateTrack>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            time = time * 2;
            SetTime(time);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            time = time / 2;
            SetTime(time);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnCar(new Vector3(0, 0, 0));

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach(GameObject car in cars)
                Destroy(car);
            foreach (GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
                Destroy(checkpoint);
            foreach (GameObject wall in GameObject.FindGameObjectsWithTag("Wall"))
                Destroy(wall);

           trackGenerator.Generate(trackLength, trackIncrementSize, trackWidth, turnRange, 0, minTurnDistance, maxTurnDistance, trackCheckpointDensity);
        }
    }

    void SetTime(float scale)
    {
        Time.timeScale = scale;
        Debug.Log("Time Set to: " + scale);
    }

    void SpawnCar(Vector3 position)
    { 
        cars.Add(Instantiate(car, position, Quaternion.identity));
        cars[cars.Count - 1].name = "Car: " + (cars.Count - 1).ToString();
        // cars[cars.Count - 1].GetComponent<CarController>().index = cars.Count - 1;
        carCount = cars.Count;
    }
}
