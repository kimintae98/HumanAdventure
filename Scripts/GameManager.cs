using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player player;
    public PoolManager poolManager;
    public Boss boss;

    public bool isPause;
    public bool isSound;
    public bool isBattle;
    public bool menuDown;
    public float playTime;
    public int stage;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public GameObject startPanel;
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject clearPanel;
    public GameObject overPanel;
    public GameObject weaponShop;
    public GameObject itemShop;
    public GameObject talkNpc;
    public GameObject map;
    public GameObject mapA;
    public GameObject mapB;
    public GameObject[] moveZones;

    public string[] enemies;
    public List<int> enemyList;
    public Transform[] enemyZones;

    public Text goldText;
    public Text stageText;
    public Text timeText;
    public Text healthText;
    public Text bossHealthText;
    public Text ammoText;
    public Text healText;
    public Text grenadeText;
    public Text soundText;

    public Image weaponImg1;
    public Image weaponImg2;
    public Image weaponImg3;
    public Image weaponImg4;
    public Image itemImg1;
    public Image itemImg2;

    public RectTransform healthGroup;
    public RectTransform healthBar;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    void Awake() // by.���� / �ʱ�ȭ
    {
        enemyList = new List<int>();
        enemies = new string[] { "EnemyA", "EnemyB", "EnemyC", "EnemyD" };
        stageText.text = "����";
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Login);
        menuDown = Input.GetKeyDown(KeyCode.Escape);
    }
    void Update() // by.���� / �ο� �� �ð� �귯 ���� �ϱ�, ESCŰ�� ������ �� �޴�â �߰��ϱ� 
    {
        if (isBattle)
            playTime += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && player.gameObject.activeSelf)
        {
            if (isPause)
                Resume();
            else
                Pause();
        }
    }
    void LateUpdate() // by.���� / UI�� ������ ������ ������Ʈ �ϱ�
    {
        // by.���� /��, �ð� 
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        timeText.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        // by.���� /ü��, ��
        healthText.text = player.health + " / " + player.maxHealth;
        if (boss != null)
            bossHealthText.text = boss.curHealth + " / " + boss.maxHealth;
        goldText.text = string.Format("{0:n0}", player.gold);
        // by.���� /���, �Һ���
        if (player.equipWeapon == null)
            ammoText.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            ammoText.text = "- / " + player.ammo;
        else
            ammoText.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        healText.text = player.healthItem.ToString();
        grenadeText.text = player.hasGrenade.ToString();
        weaponImg1.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weaponImg2.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weaponImg3.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponImg4.color = new Color(1, 1, 1, player.hasWeapons[3] ? 1 : 0);
        itemImg1.color = new Color(1, 1, 1, player.hasGrenade > 0 ? 1 : 0);
        itemImg2.color = new Color(1, 1, 1, player.healthItem > 0 ? 1 : 0);
        // by.���� /������ ��
        if (player.health <= 0)
            healthBar.localScale = new Vector3(0, 1, 1);
        else
            healthBar.localScale = new Vector3((float)player.health / player.maxHealth, 1, 1);
        if (boss != null)
        {
            if (boss.curHealth <= 0)
                bossHealthBar.localScale = new Vector3(0, 1, 1);
            else
            {
                bossHealthGroup.anchoredPosition = Vector3.down * 50;
                bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
            }
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 50;
        }
    }
    public void GameStart() // by.���� / ���� ����
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        player.gameObject.SetActive(true);
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Town);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
    public void GameOver() // by.���� / ���� ����
    {
        overPanel.SetActive(true);
        gamePanel.SetActive(false);
        player.gameObject.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
    public void GameClear() // by.���� / ���� Ŭ����
    {
        clearPanel.SetActive(true);
        gamePanel.SetActive(false);
        player.gameObject.SetActive(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }
    public void Restart() // by.���� / ���� �����
    {
        SceneManager.LoadScene(0);
    }
    public void Exit() // by.���� / ���� ������
    {
        Application.Quit();
    }
    public void Pause() // by.���� / ���� ����
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
        Time.timeScale = 0f;
        isPause = true;
    }
    public void Resume() // by.���� / ���� �簳
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        Time.timeScale = 1f;
        isPause = false;
    }
    public void Sound() // by.���� / BGM On/Off
    {
        if (isSound)
        {
            AudioManager.instance.bgmPlayer.mute = true;
            soundText.text = "BGM ON";
            isSound = false;
        }
        else
        {
            AudioManager.instance.bgmPlayer.mute = false;
            soundText.text = "BGM OFF";
            isSound = true;
        }
    }
    public void StageStart(int mapIndex) // by.���� / ���� �������� ���� ��
    {
        if (mapIndex == 1)
        {
            map.SetActive(false);
            mapA.SetActive(true);
            stageText.text = "���� ��";
            AudioManager.instance.PlayBgm(AudioManager.Bgm.Forest);
        }
        else if(mapIndex == 2)
        {
            map.SetActive(false);
            mapB.SetActive(true);
            stageText.text = "�޸��� �縷";
            AudioManager.instance.PlayBgm(AudioManager.Bgm.Desert);
        }
        stage = mapIndex;
        weaponShop.SetActive(false);
        itemShop.SetActive(false);
        talkNpc.SetActive(false);
        foreach(GameObject zone in moveZones)
            zone.SetActive(false);
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);
        isBattle = true;
        StartCoroutine(InBattle());
    }
    public void StageEnd() // by.���� / ������ ���� ��
    {
        stage = 0;
        stageText.text = "����";
        mapA.SetActive(false);
        mapB.SetActive(false);
        map.SetActive(true);
        weaponShop.SetActive(true);
        itemShop.SetActive(true);
        talkNpc.SetActive(true);
        foreach (GameObject zone in moveZones)
            zone.SetActive(true);
        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(false);
        isBattle = false;
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Town);
    }
    IEnumerator InBattle() // by.���� / ������ �� ��
    {
        if (stage == 2) // by.���� / ���� ��
        {
            enemyCntD++;
            GameObject instantEnemy = poolManager.MakeObj(enemies[3]);
            instantEnemy.transform.position = enemyZones[3].position;
            boss = instantEnemy.GetComponent<Boss>();
            boss.target = player.transform;
            boss.manager = this;
        }
        else
        {
            for (int index = 0; index < 10; index++) // by.���� / ���� ����Ʈ�� ����
            {
                int ran = Random.Range(0, 3);
                enemyList.Add(ran);
                switch (ran) // ���� ��
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
            while (enemyList.Count > 0) // by.���� / ���������� ���� ��ȯ
            {
                int ranZone = Random.Range(0, 4);
                GameObject instantEnemy = poolManager.MakeObj(enemies[enemyList[0]]);
                instantEnemy.transform.position = enemyZones[ranZone].position;
                Enemy enemy = instantEnemy.GetComponent<Enemy>();           
                enemy.target = player.transform;
                enemy.manager = this; 
                enemy.gameObject.layer = 13;
                foreach (MeshRenderer mesh in enemy.meshs)
                    mesh.material.color = Color.white;
                enemy.isDead = false;
                enemy.isChase = true;
                enemy.curHealth = enemy.maxHealth;
                enemyList.RemoveAt(0);
                yield return new WaitForSeconds(5f);
            }
        }
        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) // by.���� / ���͸� �� �׿����� üũ
        {
            yield return null;
        }
        yield return new WaitForSeconds(7f);
        boss = null;
        StageEnd();
    }
}
