using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void OnEnable() // by.���� / ON �Ǹ� ����
    {
        StartCoroutine(Explosion());
    }
    void OnDisable() // by.���� / ������ OFF �Ǹ� ���� 
    {
        meshObj.SetActive(true);
        effectObj.SetActive(false);
    }
    IEnumerator Explosion() // by.���� / ����ź ����
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hitObj in rayHits) {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
