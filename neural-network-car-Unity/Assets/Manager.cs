using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private float time = 1;

    [SerializeField] GameObject car;
    private List<GameObject> cars = new List<GameObject>();

    public int carCount;

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

        if (Input.GetKey(KeyCode.G))
        {
            SpawnCar(new Vector3(0, 1, 0));
        }
    }

    void SetTime(float scale)
    {
        Time.timeScale = scale;
        Debug.Log("Time Set to: " + scale);
    }

    void SpawnCar(Vector3 position)
    { 
        cars.Add(GameObject.Instantiate(car, position, Quaternion.identity));
        carCount = cars.Count;
    }
}
