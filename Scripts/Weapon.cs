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

    void Awake() // by.���� / �ʱ�ȭ
    {
        bullets = new string[] { "Bullet", "BulletCase" };
    }
    public void Use() // by.���� / ���� ���
    {
        if (type == Type.Melee) // by.���� / ���� ����
        {
            StopCoroutine(Swing());
            StartCoroutine(Swing());
        }
        else if (type == Type.Range && curAmmo > 0) // by.���� / ���Ÿ� ����
        {
            curAmmo--;
            StartCoroutine(Shot());
        }
    }
    IEnumerator Swing() // by.���� / �ֵθ���
    {
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = false;
    }
    IEnumerator Shot() // by.���� / �� ���
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
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse); //ȸ��
    }
}