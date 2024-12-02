using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;

public class BossUITeamInformation : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txtName;

    [SerializeField]
    private TMP_Text txtHp;

    [SerializeField]
    private Image imgHpFill;

    [SerializeField]
    private Image imaHpBack;

    [SerializeField]
    private TMP_Text txtMp;

    [SerializeField]
    private Image imgMpFill;

    [SerializeField]
    private Image imgMpBack;

    private float fullHP;
    private float curHP;

    private float fullMP;
    private float curMP;
    
    private float fillWidth = 290;
    private float fillHeight = 40;

    public void Set(PlayerStatus playerStatus)
    {

        SetName(playerStatus.PlayerName);
        SetFullHP(playerStatus.PlayerFullHp);
        SetFullMP(playerStatus.PlayerFullMp);
        SetCurHP(playerStatus.PlayerCurHp);
        SetCurMP(playerStatus.PlayerCurMp);
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

