using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
    public GameManager manager;
    public int mapIndex;

    void OnTriggerEnter(Collider other) // by.인태 / 플레이어가 포탈존에 들어오면 다른맵으로 보냄
    {
        if (other.gameObject.tag == "Player")
            manager.StageStart(mapIndex);
    }
}
