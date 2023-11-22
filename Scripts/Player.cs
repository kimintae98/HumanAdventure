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
   
    void Awake() // by.���� / �ʱ�ȭ
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        grenade = "Grenade";
    }
    void Update() // by.���� / �ൿ�� ����
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
    void FixedUpdate() // by.���� / �������� �κ� ������Ʈ
    {
        FreezeRotation();
        StopToWall();
    }
    void GetInput() // by.���� / �� ��������
    {
        hAxis = Input.GetAxisRaw("Horizontal"); // ������ ������ (����, ������)
        vAxis = Input.GetAxisRaw("Vertical"); // ������ ������ (��, �Ʒ�)
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
    void Move() // by.���� / ������
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized; // �޾ƿ� �������� Vector3 ������ ����
        if (isDodge) // �����⸦ �ϰ� �ִٸ� �� �������� �ٲ��ֱ� 
            moveVec = dodgeVec;
        if (!isBorder && isFireReady && !isDead) 
            transform.position += moveVec * speed * (runDown ? 1.5f : 1f) * Time.deltaTime; // �÷��̾���ġ�� Vector3 ���� �ο� , �ȱ�� ����
        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isRun", runDown);
    }
    void Turn() // by.���� / �ٶ󺸴� �������� ȸ��
    {
        // Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec); // ���� �������� �ٶ󺸱�
        // ���콺�� ���� ȸ��
        if (fireDown && !isDead)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)) // out : returnó�� ��ȯ���� �־��� ������ ���� 
            {
                Vector3 nextVec = rayHit.point;
                nextVec.y = 0;
                transform.LookAt(nextVec);
            }
        }
    }
    void Jump() // by.���� / ����
    {
        if(junpDown && !isJump && !isDodge && !isSwap && !isShop && !isDead) // �ߺ� �ൿ ����
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            isJump = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }
    }
    void Dodge() // by.���� / ������
    {
        if (dodgeDown && moveVec != Vector3.zero && !isDodge && !isJump && !isSwap && !isShop && !isDead) //����Ű�� ���� ������ ������ 
        {
            dodgeVec = moveVec;
            speed *= 2;
            animator.SetTrigger("doDodge");
            isDodge = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
            Invoke("DodgeOut", 0.5f);
        }
    }
    void DodgeOut() // by.���� / ������ ����
    { 
        speed *= 0.5f;
        isDodge = false;
    }
    void Attack() // by.���� / ����
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
    void Grenade() // by.���� / ����ź ��ô
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
    void Reload() // by.���� / �� ����
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
    void ReloadOut() // by.���� / ���� ����
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        int lefrAmmo = equipWeapon.maxAmmo - equipWeapon.curAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= lefrAmmo;
        isReload = false;
    } 
    void Swap() // by.���� / ������ ��ü
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
            if (equipWeapon != null) // �����ϰ��ִ� �������� �ִٸ� ���ش�
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
    void SwapOut() // by.���� / ��ü ����
    {
        isSwap = false;
    }
    void Heal() // by.���� / ü�� ȸ��
    {
        if (healDown && healthItem !=0 && !isReload && !isSwap && !isShop && !isDead)
        {
            int reHealth = maxHealth - health;
            health += reHealth;
            healthItem--;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Heal);
        }
    }
    void Interaction() // by.���� / ���� �� NPC ��ȣ�ۿ�
    {
        if (InterDown && nearObj != null && !isJump && !isDodge && !isDead)
        {
            Shop shop = nearObj.GetComponent<Shop>();
            shop.Enter(this);
            isShop = true;
        }
    }
    void OnDie() // by.���� / �÷��̾� ����
    {
        animator.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }
    void FreezeRotation() // by.���� / ������ ���� ���� ����
    {
        rigid.angularVelocity = Vector3.zero;
    }
    void StopToWall() // by.���� /�÷��̾ ���� ����Ǵ� ���� ����
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    void OnCollisionEnter(Collision collision) // by.���� / ���� �� �ٴڿ� ����� ��  �������¸� False�� �ٲٱ�
    {
        if(collision.gameObject.tag == "Floor") 
        {
            animator.SetBool("isJump", false);
            isJump = false;
        }
    }
    void OnTriggerEnter(Collider other) // by.���� / �÷��̾� �ǰ�
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
    void OnTriggerStay(Collider other) // by.���� / ������ ������ ��
    {
        if (other.tag == "Shop")
        {
            nearObj = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other) // by.���� / �������� ���� ��
    {
        if (other.tag == "Shop")
        {
            Shop shop = nearObj.GetComponent<Shop>();
            shop.Exit();
            nearObj = null;
            isShop = false;
        }
    }
    IEnumerator OnDamage(bool isBossAttack) // by.���� / �÷��̾� �ǰݽ� ȿ��
    {
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossAttack) // ���� �������ݽ� �˹�
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