using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    int time = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Plus))
        {
            time = time * 2;
            SetTime();
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            time = time / 2;
            SetTime();
        }
    }

    void SetTime()
    {
        Time.timeScale = time;
        Debug.Log("Time Set to: " + time);
    }
}
