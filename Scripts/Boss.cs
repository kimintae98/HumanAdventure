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

    void Start() // by.인태 / 오브젝트가 ON 되었을때 실행
    {
        rigid = GetComponent<Rigidbody>(); // 상속될 때 초기화는 따로 해주어야 한다.
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        nav.isStopped = true;
        StartCoroutine(Think());
    }
    void Update() // by.인태 / 보스의 행동 업데이트
    {
        if (isDead && manager.stage == 0)
        {
            StopAllCoroutines();
            return;
        }
        if (isLook) // by.인태 / 보스가 플레이어 보기
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);
    }
    IEnumerator Think() // by.인태 / 보스의 3가지 패턴
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
    IEnumerator MissileShot() // by.인태 / 미사일 날리기
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
    IEnumerator RockShot() // by.인태 / 돌 발사
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
    IEnumerator Taunt() // by.인태 / 내려찍기
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
