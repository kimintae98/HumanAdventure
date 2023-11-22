using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    void OnCollisionEnter(Collision collision) // by.���� / �Ѿ˰� ź�ǰ� �ٴڿ� �浹���� ��
    {
        if(collision.gameObject.tag == "Floor" && !isRock)
        {
            gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider other) // by.���� / �Ѿ˰� ź�ǰ� ���� �浹���� ��
    {
        if (!isMelee && other.gameObject.tag == "Wall")
        {
            gameObject.SetActive(false);
        }
    }
}
