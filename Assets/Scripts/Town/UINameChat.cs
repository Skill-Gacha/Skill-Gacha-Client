using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

struct MsgData
{
    public float Time;
    public string Text;

    public MsgData(float time, string text)
    {
        Time = time;
        Text = text;
    }
}

public class UINameChat : MonoBehaviour
{
    [SerializeField] private Mask bgMask;
    [SerializeField] private Image imgBg;
    [SerializeField] private TMP_Text txtNickname;
    [SerializeField] private TMP_Text txtChat;
    
    private Vector2 originBgSize;

    private Transform camTr;
    private string userName;
    
    private List<MsgData> _msgList = new List<MsgData>();
    private float _time;
    private float _holdingTime = 5;
    
    
    void Start()
    {
        camTr = Camera.main.transform;
    }

    
    void Update()
    {
        transform.rotation = camTr.rotation;
        
        if (_msgList.Count != 0)
        {
            _time += Time.deltaTime;
            if (_msgList[0].Time < _time)
            {
                PopText();
                SetChatText();
            }
        }
    }

    public void SetName(string userName)
    {
        this.userName = userName;
        
        txtNickname.text = userName;
        
        originBgSize = txtNickname.GetPreferredValues() + new Vector2(70, 35);
        txtNickname.rectTransform.sizeDelta = originBgSize;
        txtChat.rectTransform.sizeDelta = new Vector2(originBgSize.x, 0);
    }

    public void PushText(string text)
    {
        if (_msgList.Count == 0)
            _time = 0;
        
        _msgList.Add(new MsgData(_time + _holdingTime, text));
        
        SetChatText();
    }
    
    private void PopText()
    {
        if (_msgList.Count != 0)
            _msgList.RemoveAt(0);
    }
    
    private void ReturnToDefault()
    {
        var zeroHeight = originBgSize;
        zeroHeight.y = 0;
        
        txtChat.rectTransform.DOSizeDelta(zeroHeight, 0.2f);
        txtChat.DOFade(0, 0.1f).OnComplete(() => { txtChat.text = string.Empty; });
    }
    
    private void SetChatText()
    {
        if (_msgList.Count == 0)
        {
            ReturnToDefault();
            return;
        }

        txtChat.DOFade(1, 0.2f);

        StringBuilder chatBuilder = new StringBuilder();

        foreach (var msg in _msgList)
            chatBuilder.AppendLine(msg.Text);

        txtChat.text = chatBuilder.ToString();

        var chatWidth = Mathf.Min(txtChat.preferredWidth, 1000);

        var preferSize = txtChat.GetPreferredValues(txtChat.text, chatWidth, 60);
        txtChat.rectTransform.DOSizeDelta(new Vector2(chatWidth, preferSize.y), 0.2f);
        bgMask.enabled = true;
    }
}
