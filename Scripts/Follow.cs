using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraPos; 
    void Update() // by.인태 / 카메라 위치
    {
        transform.position = target.position + cameraPos;
    }
}
