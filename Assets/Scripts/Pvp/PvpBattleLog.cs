using DG.Tweening;
using Google.Protobuf.Protocol;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class PvpBattleLog : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLog;
    [SerializeField] private Button[] btns;
    [SerializeField] private TMP_Text[] btnTexts;
    [SerializeField] private Image imgContinue;

    private BtnInfo[] btnInfos = null;
    private bool done = false;
    private string msg;

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i + 1;
            btns[i].onClick.AddListener(() => OnClick(idx));
        }
    }

    public void Set(BattleLog battleLog)
    {
        btnInfos = (battleLog.Btns != null && battleLog.Btns.Count > 0) ? battleLog.Btns.ToArray() : null;
        Debug.Log($"Msg : {battleLog.Msg} battleLog : {battleLog.TypingAnimation}");
        SetLog(battleLog.Msg, battleLog.TypingAnimation);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (IsPvpUiScreenActive())
                return;

            HandleUserInput();
        }
    }

    private bool IsPvpUiScreenActive()
    {
        return PvpBattleManager.Instance.PvpUiScreen.gameObject.activeSelf;
    }

    private void HandleUserInput()
    {
        if (!done)
        {
            CompleteLog();
        }
        else
        {
            if (btnInfos == null)
                Response(0);
        }
    }

    private void CompleteLog()
    {
        DOTween.Kill(txtLog);
        txtLog.text = msg;
        LogDone();
    }

    public void LogDone()
    {
        done = true;

        if (btnInfos == null)
            StartContinueAnimation();
        else
            SetBtn(btnInfos);
    }

    private void StartContinueAnimation()
    {
        imgContinue.DOFade(1, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
    }

    public void SetLog(string log, bool typing = true)
    {
        done = false;
        ResetLog();

        msg = log;

        if (typing)
            AnimateText();
        else
        {
            txtLog.text = msg;
            LogDone();
        }
    }

    private void ResetLog()
    {
        DOTween.Kill(txtLog);
        txtLog.text = string.Empty;
        imgContinue.color = new Color(imgContinue.color.r, imgContinue.color.g, imgContinue.color.b, 0);
    }

    private void AnimateText()
    {
        txtLog.DOText(msg, msg.Length / 20f).SetEase(Ease.Linear).OnComplete(LogDone);
    }

    private void SetBtn(BtnInfo[] btnInfos)
    {
        HideAllButtons();
        ActivateButtons(btnInfos);
    }

    private void HideAllButtons()
    {
        foreach (var btn in btns)
            btn.gameObject.SetActive(false);
    }

    private void ActivateButtons(BtnInfo[] btnInfos)
    {
        for (int i = 0; i < btnInfos.Length && i < btns.Length; i++)
        {
            var btnInfo = btnInfos[i];
            btns[i].gameObject.SetActive(true);
            btns[i].interactable = btnInfo.Enable;
            btnTexts[i].text = btnInfo.Msg;
        }
    }

    private void OnClick(int idx)
    {
        Response(idx);
    }

    private void Response(int idx)
    {
        C_PvpPlayerResponse response = new C_PvpPlayerResponse() { ResponseCode = idx };
        GameManager.Network.Send(response);
    }
}
