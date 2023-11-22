using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject enemyAPrefab;
    public GameObject enemyBPrefab;
    public GameObject enemyCPrefab;
    public GameObject enemyDPrefab;
    public GameObject bulletPrefab;
    public GameObject bulletCasePrefab;
    public GameObject missileAPrefab;
    public GameObject missileBPrefab;
    public GameObject rockPrefab;
    public GameObject grenadePrefab;

    GameObject[] enemyA;
    GameObject[] enemyB;
    GameObject[] enemyC;
    GameObject[] enemyD;
    GameObject[] bullet;
    GameObject[] bulletCase;
    GameObject[] missileA;
    GameObject[] missileB;
    GameObject[] rock;
    GameObject[] grenade;
    void Awake() // by.인태 / 초기화
    {
        enemyA = new GameObject[10];
        enemyB = new GameObject[10];
        enemyC = new GameObject[10];
        enemyD = new GameObject[1];
        bullet = new GameObject[50];
        bulletCase = new GameObject[30];
        missileA = new GameObject[10];
        missileB = new GameObject[10];
        rock = new GameObject[10];
        grenade = new GameObject[10];
        Genarate();
    }
    void Genarate() // by.인태 / 오브젝트들 생성
    {
        for(int index = 0;  index < enemyA.Length; index++)
        {
            enemyA[index] = Instantiate(enemyAPrefab);
            enemyA[index].SetActive(false);
        }
        for (int index = 0; index < enemyB.Length; index++)
        {
            enemyB[index] = Instantiate(enemyBPrefab);
            enemyB[index].SetActive(false);
        }
        for (int index = 0; index < enemyC.Length; index++)
        {
            enemyC[index] = Instantiate(enemyCPrefab);
            enemyC[index].SetActive(false);
        }
        for (int index = 0; index < enemyD.Length; index++)
        {
            enemyD[index] = Instantiate(enemyDPrefab);
            enemyD[index].SetActive(false);
        }
        for (int index = 0; index < bullet.Length; index++)
        {
            bullet[index] = Instantiate(bulletPrefab);
            bullet[index].SetActive(false);
        }
        for (int index = 0; index < bulletCase.Length; index++)
        {
            bulletCase[index] = Instantiate(bulletCasePrefab);
            bulletCase[index].SetActive(false);
        }
        for (int index = 0; index < missileA.Length; index++)
        {
            missileA[index] = Instantiate(missileAPrefab);
            missileA[index].SetActive(false);
        }
        for (int index = 0; index < missileB.Length; index++)
        {
            missileB[index] = Instantiate(missileBPrefab);
            missileB[index].SetActive(false);
        }
        for (int index = 0; index < rock.Length; index++)
        {
            rock[index] = Instantiate(rockPrefab);
            rock[index].SetActive(false);
        }
        for (int index = 0; index < grenade.Length; index++)
        {
            grenade[index] = Instantiate(grenadePrefab);
            grenade[index].SetActive(false);
        }
    }
    public GameObject MakeObj(string type) // by.인태 / 오브젝트들 Off 되어 있는거 On 해주기 
    {
        switch(type)
        {
            case "EnemyA":
                for (int index = 0; index < enemyA.Length; index++)
                {
                    if (!enemyA[index].activeSelf)
                    {
                        enemyA[index].SetActive(true);
                        return enemyA[index];
                    }
                }
                break;
            case "EnemyB":
                for (int index = 0; index < enemyB.Length; index++)
                {
                    if (!enemyB[index].activeSelf)
                    {
                        enemyB[index].SetActive(true);
                        return enemyB[index];
                    }
                }
                break;
            case "EnemyC":
                for (int index = 0; index < enemyC.Length; index++)
                {
                    if (!enemyC[index].activeSelf)
                    {
                        enemyC[index].SetActive(true);
                        return enemyC[index];
                    }
                }
                break;
            case "EnemyD":
                for (int index = 0; index < enemyD.Length; index++)
                {
                    if (!enemyD[index].activeSelf)
                    {
                        enemyD[index].SetActive(true);
                        return enemyD[index];
                    }
                }
                break;
            case "Bullet":
                for (int index = 0; index < bullet.Length; index++)
                {
                    if (!bullet[index].activeSelf)
                    {
                        bullet[index].SetActive(true);
                        return bullet[index];
                    }
                }
                break;
            case "BulletCase":
                for (int index = 0; index < bulletCase.Length; index++)
                {
                    if (!bulletCase[index].activeSelf)
                    {
                        bulletCase[index].SetActive(true);
                        return bulletCase[index];
                    }
                }
                break;
            case "MissileA":
                for (int index = 0; index < missileA.Length; index++)
                {
                    if (!missileA[index].activeSelf)
                    {
                        missileA[index].SetActive(true);
                        return missileA[index];
                    }
                }
                break;
            case "MissileB":
                for (int index = 0; index < missileB.Length; index++)
                {
                    if (!missileB[index].activeSelf)
                    {
                        missileB[index].SetActive(true);
                        return missileB[index];
                    }
                }
                break;
            case "Rock":
                for (int index = 0; index < rock.Length; index++)
                {
                    if (!rock[index].activeSelf)
                    {
                        rock[index].SetActive(true);
                        return rock[index];
                    }
                }
                break;
            case "Grenade":
                for (int index = 0; index < grenade.Length; index++)
                {
                    if (!grenade[index].activeSelf)
                    {
                        grenade[index].SetActive(true);
                        return grenade[index];
                    }
                }
                break;
        }
        return null;
    } 
}
