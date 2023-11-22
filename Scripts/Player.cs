using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpPower;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject grenadeObj;
    public Camera followCamera;
    public GameObject nearObj;
    public Weapon equipWeapon;
    public GameManager manager;
    public PoolManager poolManager;
    public string grenade;

    public int ammo;
    public int gold;
    public int health;
    public int healthItem;
    public int hasGrenade;

    public int maxHealth;
    public bool isBossKill;

    float hAxis;
    float vAxis;

    bool runDown;
    bool junpDown;
    bool dodgeDown;
    bool swapDown1;
    bool swapDown2;
    bool swapDown3;
    bool swapDown4;
    bool InterDown;
    bool fireDown;
    bool grenadeDown;
    bool reloadDown;
    bool healDown;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;
    bool isReload;
    bool isBorder;
    bool isShop;
    bool isDead;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Rigidbody rigid;
    Animator animator;
    MeshRenderer[] meshs;
    int equipWeaponsIndex = -1;
    float fireDelay;
   
    void Awake() // by.인태 / 초기화
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        grenade = "Grenade";
    }
    void Update() // by.인태 / 행동들 실행
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Attack();
        Grenade();
        Reload();
        Swap();
        Heal();
        Interaction();
    }
    void FixedUpdate() // by.인태 / 물리적인 부분 업데이트
    {
        FreezeRotation();
        StopToWall();
    }
    void GetInput() // by.인태 / 값 가져오기
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // 수평적 움직임 (왼쪽, 오른쪽)
        vAxis = Input.GetAxisRaw("Vertical"); // 수직적 움직임 (위, 아래)
        runDown = Input.GetButton("Run");
        junpDown = Input.GetButtonDown("Jump");
        dodgeDown = Input.GetButtonDown("Dodge");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
        swapDown4 = Input.GetButtonDown("Swap4");
        InterDown = Input.GetButtonDown("Inter");
        fireDown = Input.GetButton("Fire1");
        grenadeDown = Input.GetButtonDown("Fire2");
        reloadDown = Input.GetButtonDown("Reload");
        healDown = Input.GetButtonDown("Heal");
    }
    void Move() // by.인태 / 움직임
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // 받아온 움직임을 Vector3 값으로 저장
        if (isDodge) // 구르기를 하고 있다면 그 방향으로 바꿔주기 
            moveVec = dodgeVec;
        if (!isBorder && isFireReady && !isDead) 
            transform.position += moveVec * speed * (runDown ? 1.5f : 1f) * Time.deltaTime; // 플레이어위치에 Vector3 값을 부여 , 걷기는 감속
        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isRun", runDown);
    }
    void Turn() // by.인태 / 바라보는 방향으로 회전
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec); // 가는 방향으로 바라보기
        // 마우스에 의한 회전
        if (fireDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : return처럼 반환값을 주어진 변수에 저장 
            {
                Vector3 nextVec = rayHit.point;
                nextVec.y = 0;
                transform.LookAt(nextVec);
            }
        }
    }
    void Jump() // by.인태 / 점프
    {
        if(junpDown && !isJump && !isDodge && !isSwap && !isShop && !isDead) // 중복 행동 금지
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }
    }
    void Dodge() // by.인태 / 구르기
    {
        if (dodgeDown && moveVec != Vector3.zero && !isDodge && !isJump && !isSwap && !isShop && !isDead) //방향키랑 같이 눌러야 굴러짐 
        {
            dodgeVec = moveVec;
            speed *= 2;
            animator.SetTrigger("doDodge");
            isDodge = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut() // by.인태 / 구르기 종료
    { 
        speed *= 0.5f;
        isDodge = false;
    }
    void Attack() // by.인태 / 공격
    {
        if (equipWeapon == null)
            return;
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.speed < fireDelay;
        if (fireDown && isFireReady && !isDodge && !isSwap && !isShop && !isDead)
        {
            equipWeapon.Use();
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
    }
    void Grenade() // by.인태 / 수류탄 투척
    {
        if (hasGrenade == 0)
            return;
        if(grenadeDown && !isReload && !isSwap && !isShop && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point;
                nextVec.y = 5;
                GameObject instantGrenade = poolManager.MakeObj(grenade);
                instantGrenade.transform.position = transform.position;
                instantGrenade.transform.rotation = transform.rotation;
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(nextVec * 10, ForceMode.Impulse);
                hasGrenade--;
            }
        }
    }
    void Reload() // by.인태 / 총 장전
    {
        if (equipWeapon == null || equipWeapon.type == Weapon.Type.Melee || ammo ==0) 
            return;
        if (reloadDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop && !isDead)
        {
            animator.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 2f);
        }
    } 
    void ReloadOut() // by.인태 / 장전 종료
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        int lefrAmmo = equipWeapon.maxAmmo - equipWeapon.curAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= lefrAmmo;
        isReload = false;
    } 
    void Swap() // by.인태 / 아이템 교체
    {
        if (swapDown1 && (!hasWeapons[0] || equipWeaponsIndex == 0))
            return;
        if (swapDown2 && (!hasWeapons[1] || equipWeaponsIndex == 1))
            return;
        if (swapDown3 && (!hasWeapons[2] || equipWeaponsIndex == 2))
            return;
        if (swapDown4 && (!hasWeapons[3] || equipWeaponsIndex == 3))
            return;
        int weaponIndex = -1;
        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if (swapDown3) weaponIndex = 2;
        if (swapDown4) weaponIndex = 3;
        if ((swapDown1 || swapDown2 || swapDown3 || swapDown4) && !isJump && !isDodge && !isShop && !isDead)
        {
            if (equipWeapon != null) // 소지하고있는 아이템이 있다면 꺼준다
                equipWeapon.gameObject.SetActive(false);
            equipWeaponsIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            animator.SetTrigger("doSwap");
            isSwap = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
            Invoke("SwapOut", 0.5f);
        }
    }
    void SwapOut() // by.인태 / 교체 종료
    {
        isSwap = false;
    }
    void Heal() // by.인태 / 체력 회복
    {
        if (healDown && healthItem !=0 && !isReload && !isSwap && !isShop && !isDead)
        {
            int reHealth = maxHealth - health;
            health += reHealth;
            healthItem--;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Heal);
        }
    }
    void Interaction() // by.인태 / 상점 및 NPC 상호작용
    {
        if (InterDown && nearObj != null && !isJump && !isDodge && !isDead)
        {
            Shop shop = nearObj.GetComponent<Shop>();
            shop.Enter(this);
            isShop = true;
        }
    }
    void OnDie() // by.인태 / 플레이어 죽음
    {
        animator.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }
    void FreezeRotation() // by.인태 / 스스로 도는 현상 방지
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void StopToWall() // by.인태 /플레이어가 벽에 통과되는 현상 방지
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void OnCollisionEnter(Collision collision) // by.인태 / 점프 후 바닥에 닿았을 때  점프상태를 False로 바꾸기
    {
        if(collision.gameObject.tag == "Floor") 
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
    void OnTriggerEnter(Collider other) // by.인태 / 플레이어 피격
    {
        if(other.tag == "EnemyBullet")
        {
            Bullet enemyBullet = other.GetComponent<Bullet>();
            health -= enemyBullet.damage;
            bool isBossAttack = other.name == "Boss Melee Area";
            if(other.GetComponent<Rigidbody>() != null)
            {
                other.gameObject.SetActive(false);
            }
            StartCoroutine(OnDamage(isBossAttack));
        }
    }
    void OnTriggerStay(Collider other) // by.인태 / 상점에 들어가있을 때
    {
        if (other.tag == "Shop")
        {
            nearObj = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other) // by.인태 / 상점에서 나갈 때
    {
        if (other.tag == "Shop")
        {
            Shop shop = nearObj.GetComponent<Shop>();
            shop.Exit();
            nearObj = null;
            isShop = false;
        }
    }
    IEnumerator OnDamage(bool isBossAttack) // by.인태 / 플레이어 피격시 효과
    {
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossAttack) // 보스 점프공격시 넉백
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        if (health <= 0 && !isDead)
            OnDie();
        yield return new WaitForSeconds(1f);
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if(isBossAttack)
            rigid.velocity = Vector3.zero;
    }
}