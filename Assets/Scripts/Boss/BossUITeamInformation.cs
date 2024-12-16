// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossUITeamInformation.cs ∏Æ∆—≈Õ∏µ -----

using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUITeamInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private Image imgHpBack;
    [SerializeField] private TMP_Text txtMp;
    [SerializeField] private Image imgMpFill;
    [SerializeField] private Image imgMpBack;

    private float fullHp;
    private float curHp;
    private float fullMp;
    private float curMp;

    private const float FillWidth = 290f;
    private const float FillHeight = 40f;

    public void Set(PlayerStatus playerStatus)
    {
        if (playerStatus == null) return;

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

        UpdateHpTextSize();
    }

    public void SetCurHp(float hp)
    {
        curHp = Mathf.Min(hp, fullHp);
        txtHp.text = hp.ToString("0");
        UpdateHpFill();
        UpdateHpTextSize();
    }

    public void SetFullMp(float mp, bool recover = true)
    {
        fullMp = mp;
        txtMp.text = mp.ToString("0");

        if (recover)
            SetCurMp(mp);

        UpdateMpTextSize();
    }

    public void SetCurMp(float mp)
    {
        curMp = Mathf.Min(mp, fullMp);
        txtMp.text = mp.ToString("0");
        UpdateMpFill();
        UpdateMpTextSize();
    }

    private void UpdateHpFill()
    {
        float per = curHp / fullHp;
        imgHpFill.rectTransform.sizeDelta = new Vector2(FillWidth * per, FillHeight);
    }

    private void UpdateMpFill()
    {
        float per = curMp / fullMp;
        imgMpFill.rectTransform.sizeDelta = new Vector2(FillWidth * per, FillHeight);
    }

    private void UpdateHpTextSize()
    {
        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    private void UpdateMpTextSize()
    {
        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }
}
