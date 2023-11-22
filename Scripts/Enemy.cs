using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameManager manager;
    public PoolManager poolManager;
    public enum Type { A, B, C, D };
    public Type enemyType;
    public string[] missiles;
    public int maxHealth;
    public int curHealth;
    public int gold;
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public GameObject bullet;
    public Transform target;
    public BoxCollider meleeArea;
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator animator;
    void Awake() // by.인태 / 초기화
    {
        missiles = new string[] { "MissileA", "MissileB", "Rock" };
        poolManager = FindObjectOfType<PoolManager>();
    }
    void Start() // by.인태 / 오브젝트가 ON 되면 실행
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        if (enemyType != Type.D)
            Invoke("ChaseStart", 2);
    }
    void Update() // by.인태 / 타겟을 따라가도록 인공지능 사용
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }
    void FixedUpdate() // by.인태 / 물리 업데이트
    {
        Targeting();
        FreezeVelocity();
    }
    void ChaseStart() // by.인태 / 플레이어 추적 시작
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }
    void Targeting() // by.인태 / 몬스터의 플레이어 탐지
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 0;
            float targetRange = 0;
            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.B:
                    targetRadius = 1f;
                    targetRange = 10f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 20f;
                    break;
            }
            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }
    void FreezeVelocity() // by.인태 / 힘 초기화
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void OnTriggerEnter(Collider other) // by.인태 / 피격
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
            Debug.Log(curHealth);
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            other.gameObject.SetActive(false);
            StartCoroutine(OnDamage(reactVec));
            Debug.Log(curHealth);
        }
    }
    public void HitByGrenade(Vector3 explosionPos) // by.인태 / 수류탄에 맞았을때
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position + explosionPos;
        StartCoroutine(OnDamage(reactVec));
    }
    IEnumerator OnDamage(Vector3 reactVec) // by.인태 / 피격 효과
    {
        foreach(MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; // by.인태 / 피격시 빨간색으로 잠깐 변함
        yield return new WaitForSeconds(0.1f);
        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else // 죽었을떄
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14; // by.인태 / EnemyDead 상태로 만들어서 충돌이 없게한다
            isDead = true;
            isChase = false;
            //nav.enabled = false;
            animator.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            player.gold += gold;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Kill);
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse); // 넉백
            yield return new WaitForSeconds(3f);
            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    player.isBossKill = true;
                    break;
            }
            gameObject.SetActive(false);
        }
    }
    IEnumerator Attack() // by.인태 / 몬스터의 공격
    {
        isChase = false;
        isAttack = true;
        switch (enemyType)
        {
            case Type.A:
                animator.SetBool("isAttack", true);
                yield return new WaitForSeconds(0.2f);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Missile);
                GameObject instantBullet = poolManager.MakeObj(missiles[0]);
                instantBullet.transform.position = transform.position;
                instantBullet.transform.rotation = transform.rotation;
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;
                yield return new WaitForSeconds(2f);
                break;
        }
        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }
}
