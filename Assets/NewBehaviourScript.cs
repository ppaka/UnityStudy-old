using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject prefab;

    private void Start()
    {
        InvokeRepeating("CreateCube", 1, 1);
    }

    private void Update()
    {
    }

    public void CreateCube()
    {
        Instantiate(prefab).transform.position += Vector3.up * 10;
                                //0,0,0 + 0,10,0         //0,1,0 * 10 = 0,10,0
    }
}
