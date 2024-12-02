using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIPlayerInformation : MonoBehaviour
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

    private float fullHp;
    private float curHp;

    private float fullMp;
    private float curMp;

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
        SetFullHp(playerStatus.PlayerFullHp);
        SetFullMp(playerStatus.PlayerFullMp);
        SetCurHp(playerStatus.PlayerCurHp);
        SetCurMp(playerStatus.PlayerCurMp);
        SetElement(playerStatus.PlayerClass);
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
    }

    public void SetFullHp(float hp, bool recover = true)
    {
        fullHp = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHp(hp);
        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public void SetCurHp(float hp)
    {
        curHp = Mathf.Min(hp, fullHp);
        txtHp.text = hp.ToString("0");
        float per = curHp/fullHp;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public void SetFullMp(float mp, bool recover = true)
    {
        fullMp = mp;
        txtMp.text = mp.ToString("0");

        if (recover)
            SetCurMp(mp);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }

    public void SetCurMp(float mp)
    {
        curMp = Mathf.Min(mp, fullHp);
        txtMp.text = mp.ToString("0");

        float per = curMp/fullMp;
        imgMpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);

        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }
}
