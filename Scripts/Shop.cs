using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    Player enterPlayer;
    public GameManager gameManager;
    public RectTransform uiGroup;
    public Animator animator;
    public Text talkText;
    public Text nextText;
    public int shopIndex;
    public int talkIndex;
    public int[] itemPrice;
    public string[] talkData;
     
    public void Enter(Player player) // by.���� / ���� ����
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
        if (enterPlayer.isBossKill && shopIndex == 2)
        {
            talkText.text = "�츮 ������ �����༭ ����!";
            nextText.text = "�Ϸ�";
        }
        else
            talkText.text = talkData[0];
    }
    public void Exit() // by.���� / �������� ������
    {
        animator.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }
    public void Buy(int index) // by.���� / ���� ����
    {
        int price = itemPrice[index];
        if (price > enterPlayer.gold || (shopIndex == 0 && enterPlayer.hasWeapons[index] == true)) {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Cancle);
            return;
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        enterPlayer.gold -= price;
        if(shopIndex == 0) // ���� ����
            enterPlayer.hasWeapons[index] = true;
        else if(shopIndex == 1) // �Һ� ����
        {
            switch (index)
            {
                case 0:
                    enterPlayer.healthItem++;
                break;
                case 1:
                    enterPlayer.ammo += 100;
                break;
                case 2:
                    enterPlayer.hasGrenade++;
                break;
                case 3:
                    enterPlayer.ammo += 1000;
                break;
            }
        }
    }
    public void Talking() // by.���� / Npc ���ϱ�
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        if (shopIndex == 2 && !enterPlayer.isBossKill)
        {
            if (talkIndex == talkData.Length)
            {
                uiGroup.anchoredPosition = Vector3.down * 1000;
                talkIndex = 1;
                nextText.text = "����";
                return;
            }
            if (talkIndex == talkData.Length - 1)
                nextText.text = "�Ϸ�";
            talkText.text = talkData[talkIndex];
            talkIndex++;
        }
        else if (shopIndex == 2 && enterPlayer.isBossKill)
            gameManager.GameClear();
    }
    IEnumerator Talk() // by.���� / ��ȭ �ؽ�Ʈ ��ü
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
