using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private TMP_Text txtMsg;

    [SerializeField] private TMP_Text txtContinue;
    
    private bool done = false;
    private string msg;

    public void Set(ScreenText sText)
    {
        if(gameObject.activeSelf == false)
            gameObject.SetActive(true);
        
        SetText(sText.Msg, sText.TypingAnimation);
        
        if (sText.TextColor != null)
            SetTextColor((byte)sText.TextColor.R, (byte)sText.TextColor.G, (byte)sText.TextColor.B);
        
        if (sText.ScreenColor != null)
            SetBgColor((byte)sText.ScreenColor.R, (byte)sText.ScreenColor.G, (byte)sText.ScreenColor.B);
        
        if (sText.Alignment != null)
            SetTextAlign(sText.Alignment.X, sText.Alignment.Y);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            if (done == false)
            {
                DOTween.KillAll();
                txtMsg.text = msg;
                ScreenDone();
            }
            else
            {
                C_PlayerResponse response = new C_PlayerResponse() { ResponseCode = 0 };
                GameManager.Network.Send(response);
            }
        }
    }

    private void OnDisable()
    {
        ResetText();
        done = false;
    }

    void ResetText()
    {
        txtMsg.text = string.Empty;
        txtContinue.alpha = 0;
        DOTween.KillAll();
    }

    public void ScreenDone()
    {
        done = true;
        txtContinue.DOFade(1, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
        Invoke("Test", 5);
    }


    public void SetText(string msg, bool typing = true)
    {
        done = false;
        this.msg = msg;
        
        ResetText();
        
        if(typing)
            txtMsg.DOText(msg, msg.Length/10).SetEase(Ease.Linear).OnComplete(ScreenDone);
        else
        {
            txtMsg.text = msg;
            ScreenDone();
        }
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
        txtMsg.alignment = TextAlignmentOptions.Midline;
        switch (x)
        {
            case 0:
                switch (y)
                {
                    case 0:
                        txtMsg.alignment = TextAlignmentOptions.TopLeft;
                        break;
                    case 1:
                        txtMsg.alignment = TextAlignmentOptions.MidlineLeft;
                        break;
                    case 2:
                        txtMsg.alignment = TextAlignmentOptions.BottomLeft;
                        break;
                }
                break;
            
            case 1:
                switch (y)
                {
                    case 0:
                        txtMsg.alignment = TextAlignmentOptions.Top;
                        break;
                    case 1:
                        txtMsg.alignment = TextAlignmentOptions.Midline;
                        break;
                    case 2:
                        txtMsg.alignment = TextAlignmentOptions.Bottom;
                        break;
                }
                break;
            
            case 2:
                switch (y)
                {
                    case 0:
                        txtMsg.alignment = TextAlignmentOptions.TopRight;
                        break;
                    case 1:
                        txtMsg.alignment = TextAlignmentOptions.MidlineRight;
                        break;
                    case 2:
                        txtMsg.alignment = TextAlignmentOptions.BottomRight;
                        break;
                }
                break;
        }
    }
}
