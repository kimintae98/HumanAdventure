using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
    public GameManager manager;
    public int mapIndex;

    void OnTriggerEnter(Collider other) // by.���� / �÷��̾ ��Ż���� ������ �ٸ������� ����
    {
        if (other.gameObject.tag == "Player")
            manager.StageStart(mapIndex);
    }
}
