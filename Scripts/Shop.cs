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
     
    public void Enter(Player player) // by.인태 / 상점 출입
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
        if (enterPlayer.isBossKill && shopIndex == 2)
        {
            talkText.text = "우리 마을을 구해줘서 고마워!";
            nextText.text = "완료";
        }
        else
            talkText.text = talkData[0];
    }
    public void Exit() // by.인태 / 상점에서 나가기
    {
        animator.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }
    public void Buy(int index) // by.인태 / 물건 구입
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
        if(shopIndex == 0) // 무기 상점
            enterPlayer.hasWeapons[index] = true;
        else if(shopIndex == 1) // 소비 상점
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
    public void Talking() // by.인태 / Npc 말하기
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        if (shopIndex == 2 && !enterPlayer.isBossKill)
        {
            if (talkIndex == talkData.Length)
            {
                uiGroup.anchoredPosition = Vector3.down * 1000;
                talkIndex = 1;
                nextText.text = "다음";
                return;
            }
            if (talkIndex == talkData.Length - 1)
                nextText.text = "완료";
            talkText.text = talkData[talkIndex];
            talkIndex++;
        }
        else if (shopIndex == 2 && enterPlayer.isBossKill)
            gameManager.GameClear();
    }
    IEnumerator Talk() // by.인태 / 대화 텍스트 교체
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
