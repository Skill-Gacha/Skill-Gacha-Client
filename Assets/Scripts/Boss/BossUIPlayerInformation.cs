// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossUIPlayerInformation.cs 리팩터링 -----

using System;
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

    private float fullHp;
    private float curHp;
    private float fullMp;
    private float curMp;

    private const float FillWidth = 634f;
    private const float FillHeight = 40f;

    private string[] elementList = { "전기 속성", "땅 속성", "풀 속성", "불 속성", "물 속성" };

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Set(PlayerStatus playerStatus)
    {
        if (playerStatus == null) return;

        SetName(playerStatus.PlayerName);
        SetFullHp(playerStatus.PlayerFullHp);
        SetFullMp(playerStatus.PlayerFullMp);
        SetCurHp(playerStatus.PlayerCurHp);
        SetCurMp(playerStatus.PlayerCurMp);
        SetElement(playerStatus.PlayerClass);
    }

    public void SetElement(int element)
    {
        int elementIndex = element - 1001;
        if (IsValidElementIndex(elementIndex))
        {
            txtLv.text = elementList[elementIndex];
            imgElement.sprite = elements[elementIndex];
        }
        else
        {
            Debug.LogWarning($"Invalid element index: {elementIndex}");
        }
    }

    private bool IsValidElementIndex(int index)
    {
        return index >= 0 && index < elements.Length && index < elementList.Length;
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
