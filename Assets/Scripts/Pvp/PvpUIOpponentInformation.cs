using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private readonly float fillWidth = 230f;
    private readonly float fillHeight = 30f;

    private void Start()
    {
        camTr = Camera.main.transform;
    }

    public void Set(PlayerStatus playerStatus)
    {
        if (playerStatus == null) return;

        SetName(playerStatus.PlayerName);
        SetFullHP(playerStatus.PlayerFullHp, false);
        SetCurHP(playerStatus.PlayerCurHp);
        SetElement(playerStatus.PlayerClass);
    }

    private void SetName(string nickname)
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
        UpdateHpFill();
    }

    private void UpdateHpFill()
    {
        float per = curHP / fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);
    }

    public void SetElement(int element)
    {
        int elementIndex = element - 1001;
        if (elementIndex >= 0 && elementIndex < elementSprite.Length)
        {
            imageElement.sprite = elementSprite[elementIndex];
        }
    }
}
