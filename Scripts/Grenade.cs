using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    void OnEnable() // by.인태 / ON 되면 실행
    {
        StartCoroutine(Explosion());
    }
    void OnDisable() // by.인태 / 터지고 OFF 되면 실행 
    {
        meshObj.SetActive(true);
        effectObj.SetActive(false);
    }
    IEnumerator Explosion() // by.인태 / 수류탄 폭팔
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
