using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform misPortA;
    public Transform misPortB;
    public bool isLook;
    Vector3 lookVec;
    Vector3 tauntVec;

    void Start() // by.���� / ������Ʈ�� ON �Ǿ����� ����
    {
        rigid = GetComponent<Rigidbody>(); // ��ӵ� �� �ʱ�ȭ�� ���� ���־�� �Ѵ�.
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        nav.isStopped = true;
        StartCoroutine(Think());
    }
    void Update() // by.���� / ������ �ൿ ������Ʈ
    {
        if (isDead && manager.stage == 0)
        {
            StopAllCoroutines();
            return;
        }
        if (isLook) // by.���� / ������ �÷��̾� ����
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
    }
    IEnumerator Think() // by.���� / ������ 3���� ����
    {
        yield return new WaitForSeconds(0.1f);
        int ranAction = Random.Range(0, 3);
        switch(ranAction)
        {
            case 0:
                StartCoroutine(MissileShot());
            break; 
            case 1:
                StartCoroutine(RockShot());
            break;
            case 2:
                StartCoroutine(Taunt());
            break;
        }
    }
    IEnumerator MissileShot() // by.���� / �̻��� ������
    {
        animator.SetTrigger("doShot");
        yield return new WaitForSeconds(2.5f);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Missile);
        // GameObject instantMissileA = Instantiate(missile, misPortA.position, misPortA.rotation);
        GameObject instantMissileA = poolManager.MakeObj(missiles[1]);
        instantMissileA.transform.position = misPortA.position;
        instantMissileA.transform.rotation = misPortA.rotation;
        Bullet bossMissileA  = instantMissileA.GetComponent<Bullet>();
        Rigidbody rigidBulletA = bossMissileA.GetComponent<Rigidbody>();
        rigidBulletA.velocity = transform.forward * 20;
        //    GameObject instantMissileB = Instantiate(missile, misPortB.position, misPortB.rotation);
        GameObject instantMissileB = poolManager.MakeObj(missiles[1]);
        instantMissileB.transform.position = misPortB.position;
        instantMissileB.transform.rotation = misPortB.rotation;
        Bullet bossMissileB = instantMissileB.GetComponent<Bullet>();
        Rigidbody rigidBulletB = bossMissileB.GetComponent<Rigidbody>();
        rigidBulletB.velocity = transform.forward * 20;
        StartCoroutine(Think());
    }
    IEnumerator RockShot() // by.���� / �� �߻�
    {
        yield return new WaitForSeconds(2f);
        isLook = false;
        animator.SetTrigger("doBigShot");
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Rock);
        //Instantiate(bullet, transform.position, transform.rotation);
        GameObject instantRock = poolManager.MakeObj(missiles[2]);
        instantRock.transform.position = transform.position;
        instantRock.transform.rotation = transform.rotation;
        Bullet bossRock = instantRock.GetComponent<Bullet>();
        Rigidbody rigidRock = bossRock.GetComponent<Rigidbody>();
        rigidRock.velocity = transform.forward * 50;
        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt() // by.���� / �������
    {
        tauntVec = target.position + lookVec;
        isLook = false;
        nav.isStopped = false;
        animator.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.1f);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Taunt);
        yield return new WaitForSeconds(0.4f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        StartCoroutine(Think());
    }
}
