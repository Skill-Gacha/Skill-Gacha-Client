using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PvpUIScreen : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private TMP_Text txtMsg;
    [SerializeField] private TMP_Text txtContinue;

    private bool done = false;
    private string msg;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            HandleUserInput();
        }
    }

    private void HandleUserInput()
    {
        if (!done)
        {
            CompleteText();
        }
        else
        {
            SendResponse();
        }
    }

    private void CompleteText()
    {
        DOTween.Kill(txtMsg);
        txtMsg.text = msg;
        ScreenDone();
    }

    private void SendResponse()
    {
        C_PvpPlayerResponse response = new C_PvpPlayerResponse { ResponseCode = 0 };
        GameManager.Network.Send(response);
    }

    private void OnDisable()
    {
        ResetScreen();
    }

    private void ResetScreen()
    {
        txtMsg.text = string.Empty;
        txtContinue.alpha = 0;
        DOTween.Kill(txtMsg);
    }

    public void Set(ScreenText sText)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        ApplyScreenSettings(sText);
    }

    private void ApplyScreenSettings(ScreenText sText)
    {
        SetText(sText.Msg, sText.TypingAnimation);

        if (sText.TextColor != null)
            SetTextColor((byte)sText.TextColor.R, (byte)sText.TextColor.G, (byte)sText.TextColor.B);

        if (sText.ScreenColor != null)
            SetBgColor((byte)sText.ScreenColor.R, (byte)sText.ScreenColor.G, (byte)sText.ScreenColor.B);

        if (sText.Alignment != null)
            SetTextAlign(sText.Alignment.X, sText.Alignment.Y);
    }

    public void ScreenDone()
    {
        done = true;
        StartContinueAnimation();
    }

    private void StartContinueAnimation()
    {
        txtContinue.DOFade(1, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
    }

    public void SetText(string message, bool typing = true)
    {
        done = false;
        msg = message;
        ResetScreen();

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
        txtMsg.alignment = DetermineTextAlignment(x, y);
    }

    private TextAlignmentOptions DetermineTextAlignment(int x, int y)
    {
        switch (x)
        {
            case 0:
                return y switch
                {
                    0 => TextAlignmentOptions.TopLeft,
                    1 => TextAlignmentOptions.MidlineLeft,
                    2 => TextAlignmentOptions.BottomLeft,
                    _ => TextAlignmentOptions.MidlineLeft
                };
            case 1:
                return y switch
                {
                    0 => TextAlignmentOptions.Top,
                    1 => TextAlignmentOptions.Midline,
                    2 => TextAlignmentOptions.Bottom,
                    _ => TextAlignmentOptions.Midline
                };
            case 2:
                return y switch
                {
                    0 => TextAlignmentOptions.TopRight,
                    1 => TextAlignmentOptions.MidlineRight,
                    2 => TextAlignmentOptions.BottomRight,
                    _ => TextAlignmentOptions.MidlineRight
                };
            default:
                return TextAlignmentOptions.Midline;
        }
    }
}
