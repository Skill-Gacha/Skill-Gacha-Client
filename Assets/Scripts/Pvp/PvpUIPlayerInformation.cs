using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PvpUIPlayerInformation : MonoBehaviour
{
    [SerializeField] private TMP_Text txtElement;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private Image imgHpBack;
    [SerializeField] private TMP_Text txtMp;
    [SerializeField] private Image imgElement;
    [SerializeField] private Image imgMpFill;
    [SerializeField] private Image imgMpBack;
    [SerializeField] private Sprite[] elementSprite;

    private readonly string[] elementList = { "전기 속성", "땅 속성", "풀 속성", "불 속성", "물 속성" };

    private float fullHP;
    private float curHP;
    private float fullMP;
    private float curMP;

    private readonly float fillWidth = 634f;
    private readonly float fillHeight = 40f;

    public void Set(PlayerStatus playerStatus)
    {
        if (playerStatus == null) return;

        SetName(playerStatus.PlayerName);
        SetElement(playerStatus.PlayerClass);
        SetFullHP(playerStatus.PlayerFullHp);
        SetFullMP(playerStatus.PlayerFullMp);
        SetCurHP(playerStatus.PlayerCurHp);
        SetCurMP(playerStatus.PlayerCurMp);
    }

    private void SetName(string nickname)
    {
        txtName.text = nickname;
    }

    public void SetElement(int element)
    {
        int elementIndex = element - 1001;
        if (elementIndex >= 0 && elementIndex < elementSprite.Length && elementIndex < elementList.Length)
        {
            imgElement.sprite = elementSprite[elementIndex];
            txtElement.text = elementList[elementIndex];
        }
    }

    public void SetFullHP(float hp, bool recover = true)
    {
        fullHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHP(hp);

        UpdateHpTextSize();
    }

    public void SetCurHP(float hp)
    {
        curHP = Mathf.Min(hp, fullHP);
        txtHp.text = hp.ToString("0");
        UpdateHpFill();
        UpdateHpTextSize();
    }

    private void UpdateHpFill()
    {
        float per = curHP / fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);
    }

    private void UpdateHpTextSize()
    {
        txtHp.rectTransform.sizeDelta = new Vector2(txtHp.preferredWidth + 50, 40);
    }

    public void SetFullMP(float mp, bool recover = true)
    {
        fullMP = mp;
        txtMp.text = mp.ToString("0");

        if (recover)
            SetCurMP(mp);

        UpdateMpTextSize();
    }

    public void SetCurMP(float mp)
    {
        curMP = Mathf.Min(mp, fullMP);
        txtMp.text = mp.ToString("0");
        UpdateMpFill();
        UpdateMpTextSize();
    }

    private void UpdateMpFill()
    {
        float per = curMP / fullMP;
        imgMpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);
    }

    private void UpdateMpTextSize()
    {
        txtMp.rectTransform.sizeDelta = new Vector2(txtMp.preferredWidth + 50, 40);
    }
}
