using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaidUIPlayerInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv;
    [SerializeField] private TMP_Text txtName;
    
    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private Image imgHpBack;
    [SerializeField] private Image imgElement;

    [SerializeField] private TMP_Text txtMp;
    [SerializeField] private Image imgMpFill;
    [SerializeField] private Image imgMpBack;

    [SerializeField] public Sprite[] elements;

    // private Image elementIcon;
    
    private float fullHP;
    private float curHP;

    private float fullMP;
    private float curMP;
    
    private float fillWidth = 634;
    private float fillHeight = 40;
    
    private string[] elementList = {"전기 속성", "땅 속성", "풀 속성", "불 속성", "물 속성"};

    SpriteRenderer spriteRenderer;

    void Start()
    {
       spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetElement(int element)
    {
        int elementIndex = element - 1001;
        txtLv.text = $"{elementList[elementIndex]}";
        imgElement.sprite = elements[elementIndex];
    }

    public void Set(PlayerStatus playerStatus)
    {
        SetName(playerStatus.PlayerName);
        // SetLevel(playerStatus.PlayerClass);
        SetFullHP(playerStatus.PlayerFullHp);
        SetFullMP(playerStatus.PlayerFullMp);
        SetCurHP(playerStatus.PlayerCurHp);
        SetCurMP(playerStatus.PlayerCurMp);
        SetElement(playerStatus.PlayerClass);
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
    }

    public void SetFullHP(float hp, bool recover = true)
    {
        fullHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHP(hp);
        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public void SetCurHP(float hp)
    {
        curHP = Mathf.Min(hp, fullHP);
        txtHp.text = hp.ToString("0");
        float per = curHP/fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public void SetFullMP(float mp, bool recover = true)
    {
        fullMP = mp;
        txtMp.text = mp.ToString("0");

        if (recover)
            SetCurMP(mp);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }

    public void SetCurMP(float mp)
    {
        curMP = Mathf.Min(mp, fullHP);
        txtMp.text = mp.ToString("0");

        float per = curMP/fullMP;
        imgMpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }
}
