using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public void Initialize(float width) 
    {
        transform.localScale = new Vector3(width * 1.5f, 1, 1);
    }
}
