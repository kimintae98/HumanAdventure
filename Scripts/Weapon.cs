using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }
    public Type type;
    public int damage;
    public int maxAmmo;
    public int curAmmo;
    public float speed;
    public string[] bullets;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;
    public PoolManager poolManager;
    public Player player;

    void Awake() // by.인태 / 초기화
    {
        bullets = new string[] { "Bullet", "BulletCase" };
    }
    public void Use() // by.인태 / 무기 사용
    {
        if (type == Type.Melee) // by.인태 / 근접 공격
        {
            StopCoroutine(Swing());
            StartCoroutine(Swing());
        }
        else if (type == Type.Range && curAmmo > 0) // by.인태 / 원거리 공격
        {
            curAmmo--;
            StartCoroutine(Shot());
        }
    }
    IEnumerator Swing() // by.인태 / 휘두르기
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;
    }
    IEnumerator Shot() // by.인태 / 총 쏘기
    {
        GameObject instantBullet = poolManager.MakeObj(bullets[0]);
        instantBullet.transform.position = bulletPos.position;
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;
        yield return new WaitForSeconds(0.1f);
        GameObject instantCase = poolManager.MakeObj(bullets[1]);
        instantCase.transform.position = bulletCasePos.position;
        Rigidbody caseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, -3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //회전
    }
}