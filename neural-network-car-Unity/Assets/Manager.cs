using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    private float time = 1;

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
    }

    void SetTime(float scale)
    {
        Time.timeScale = scale;
        Debug.Log("Time Set to: " + scale);
    }
}
