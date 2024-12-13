// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossBattleLog.cs ∏Æ∆—≈Õ∏µ -----

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class BossBattleLog : MonoBehaviour
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
        SetLog(battleLog.Msg, battleLog.TypingAnimation);
    }

    private void Update()
    {
        if (IsInputTriggered())
        {
            HandleInput();
        }
    }

    private bool IsInputTriggered()
    {
        return Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0);
    }

    private void HandleInput()
    {
        if (BossManager.Instance.BossUiScreen.gameObject.activeSelf)
            return;

        if (!done)
        {
            DOTween.KillAll();
            txtLog.text = msg;
            LogDone();
        }
        else
        {
            if (btnInfos == null)
                Response(0);
        }
    }

    public void LogDone()
    {
        done = true;

        if (btnInfos == null)
            StartContinueAnimation();
        else
            SetButtons(btnInfos);
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
        DOTween.KillAll();
        txtLog.text = string.Empty;
        imgContinue.color = new Color(imgContinue.color.r, imgContinue.color.g, imgContinue.color.b, 0);
    }

    private void AnimateText()
    {
        txtLog.DOText(msg, msg.Length / 20f).SetEase(Ease.Linear).OnComplete(LogDone);
    }

    private void SetButtons(BtnInfo[] btnInfos)
    {
        DeactivateAllButtons();
        ActivateButtons(btnInfos);
    }

    private void DeactivateAllButtons()
    {
        foreach (var btn in btns)
            btn.gameObject.SetActive(false);
    }

    private void ActivateButtons(BtnInfo[] btnInfos)
    {
        for (int i = 0; i < btnInfos.Length; i++)
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
        C_BossPlayerResponse response = new C_BossPlayerResponse() { ResponseCode = idx };
        GameManager.Network.Send(response);
    }
}
