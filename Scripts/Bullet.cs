using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    void OnCollisionEnter(Collision collision) // by.인태 / 총알과 탄피가 바닥에 충돌했을 때
    {
        if(collision.gameObject.tag == "Floor" && !isRock)
        {
            gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other) // by.인태 / 총알과 탄피가 벽에 충돌했을 때
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            gameObject.SetActive(false);
        }
    }
}
