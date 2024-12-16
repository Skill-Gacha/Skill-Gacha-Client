// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossUIScreen.cs 리팩터링 -----

using System;
using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIScreen : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private TMP_Text txtMsg;
    [SerializeField] private TMP_Text txtContinue;

    private bool done = false;
    private string msg;

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
        if (!done)
        {
            DOTween.KillAll();
            txtMsg.text = msg;
            ScreenDone();
        }
        else
        {
            SendResponse(0);
        }
    }

    private void SendResponse(int responseCode)
    {
        C_BossPlayerResponse response = new C_BossPlayerResponse() { ResponseCode = responseCode };
        GameManager.Network.Send(response);
    }

    private void OnDisable()
    {
        ResetText();
        done = false;
    }

    private void ResetText()
    {
        txtMsg.text = string.Empty;
        txtContinue.alpha = 0;
        DOTween.KillAll();
    }

    public void ScreenDone()
    {
        done = true;
        StartContinueAnimation();
        // Invoke("Test", 5); // 필요 시 활성화
    }

    private void StartContinueAnimation()
    {
        txtContinue.DOFade(1, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
    }

    public void Set(ScreenText sText)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        ApplyScreenText(sText);
    }

    private void ApplyScreenText(ScreenText sText)
    {
        SetText(sText.Msg, sText.TypingAnimation);
        SetOptionalProperties(sText);
    }

    private void SetOptionalProperties(ScreenText sText)
    {
        if (sText.TextColor != null)
            SetTextColor((byte)sText.TextColor.R, (byte)sText.TextColor.G, (byte)sText.TextColor.B);

        if (sText.ScreenColor != null)
            SetBgColor((byte)sText.ScreenColor.R, (byte)sText.ScreenColor.G, (byte)sText.ScreenColor.B);

        if (sText.Alignment != null)
            SetTextAlign(sText.Alignment.X, sText.Alignment.Y);
    }

    public void SetText(string message, bool typing = true)
    {
        done = false;
        msg = message;

        ResetText();

        if (typing)
            AnimateText();
        else
        {
            txtMsg.text = msg;
            ScreenDone();
        }
    }

    private void AnimateText()
    {
        txtMsg.DOText(msg, msg.Length / 10f).SetEase(Ease.Linear).OnComplete(ScreenDone);
    }

    public void SetBgColor(byte r, byte g, byte b)
    {
        bg.color = new Color32(r, g, b, 255);
    }

    public void SetTextColor(byte r, byte g, byte b)
    {
        txtMsg.color = new Color32(r, g, b, 255);
    }

    public void SetTextAlign(int x, int y)
    {
        txtMsg.alignment = GetTextAlignment(x, y);
    }

    private TextAlignmentOptions GetTextAlignment(int x, int y)
    {
        if (x == 0)
        {
            return y switch
            {
                0 => TextAlignmentOptions.TopLeft,
                1 => TextAlignmentOptions.MidlineLeft,
                2 => TextAlignmentOptions.BottomLeft,
                _ => TextAlignmentOptions.MidlineLeft
            };
        }
        else if (x == 1)
        {
            return y switch
            {
                0 => TextAlignmentOptions.Top,
                1 => TextAlignmentOptions.Midline,
                2 => TextAlignmentOptions.Bottom,
                _ => TextAlignmentOptions.Midline
            };
        }
        else if (x == 2)
        {
            return y switch
            {
                0 => TextAlignmentOptions.TopRight,
                1 => TextAlignmentOptions.MidlineRight,
                2 => TextAlignmentOptions.BottomRight,
                _ => TextAlignmentOptions.MidlineRight
            };
        }
        else
        {
            return TextAlignmentOptions.Midline;
        }
    }
}
