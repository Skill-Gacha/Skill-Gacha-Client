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

    private float fullHp;
    private float curHp;

    private float fullMp;
    private float curMp;
    
    private float fillWidth = 290;
    private float fillHeight = 40;

    public void Set(PlayerStatus playerStatus)
    {

        SetName(playerStatus.PlayerName);
        SetFullHp(playerStatus.PlayerFullHp);
        SetFullMp(playerStatus.PlayerFullMp);
        SetCurHp(playerStatus.PlayerCurHp);
        SetCurMp(playerStatus.PlayerCurMp);
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

