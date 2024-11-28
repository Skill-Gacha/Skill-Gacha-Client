using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class PvpUIOpponentInformation : MonoBehaviour
{
    private Transform camTr;

    [SerializeField] private GameObject checkArrow;


    [SerializeField] private Image imageElement;

    [SerializeField] private Sprite[] elementSprite;

    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgNameBg;
    [SerializeField] private Image imgHpFill;

    private float fullHP;
    private float curHP;

    private float fillWidth = 230;
    private float fillHeight = 30;

    private void Start()
    {
        camTr = Camera.main.transform;
    }

    public void Set(PlayerStatus playerStatus)
    {
        SetName(playerStatus.PlayerName);
        SetFullHP(playerStatus.PlayerFullHp);
        SetCurHP(playerStatus.PlayerCurHp);
        SetElement(playerStatus.PlayerClass);
    }

    public void SetElement(int element)
    {
        int elementIndex = element - 1001;
        imageElement.sprite = elementSprite[elementIndex];
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
        // imgNameBg.rectTransform.sizeDelta = new Vector2(txtName.preferredWidth + 50, 50);
    }

    public void SetFullHP(float hp, bool recover = true)
    {
        fullHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHP(hp);
    }

    public void SetCurHP(float hp)
    {
        curHP = Mathf.Min(hp, fullHP);
        txtHp.text = hp.ToString("0");
        float per = curHP / fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);
    }
}