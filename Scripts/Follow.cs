using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 cameraPos; 
    void Update() // by.���� / ī�޶� ��ġ
    {
        transform.position = target.position + cameraPos;
    }
}
