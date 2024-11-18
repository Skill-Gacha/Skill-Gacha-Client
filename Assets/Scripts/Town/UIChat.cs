using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField] private Scrollbar scroll;
    [SerializeField] private RectTransform rectBg;
    
    [SerializeField] private Transform chatItemRoot;
    [SerializeField] private TMP_Text txtChatItemBase;
    
    [SerializeField] private TMP_InputField inputChat;
    [SerializeField] private Button btnSend;
    
    [SerializeField] private Button btnToggle;
    [SerializeField] private Transform icon;
    
    [SerializeField] private Transform alarm;
    
    private float _baseChatItemWidth;

    private Player player;

    private bool isOpen = true;
    
    
    private void Start()
    {
        _baseChatItemWidth = txtChatItemBase.rectTransform.sizeDelta.x;

        player = TownManager.Instance.myPlayer;
        
        btnSend.onClick.AddListener(SendMessage);
        btnToggle.onClick.AddListener(ToggleChatWindow);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputChat.IsActive())
            {
                SendMessage();
            }
            else
            {
                inputChat.ActivateInputField();
            }
        }
    }

    private void ToggleChatWindow()
    {
        if (isOpen)
        {
            rectBg.DOSizeDelta(new Vector2(100,40), 0.3f);
            icon.DORotate(new Vector3(0, 0, 180), 0.3f);
        }
        else
        {
            alarm.gameObject.SetActive(false);
            rectBg.DOSizeDelta(new Vector2(550,500), 0.3f);
            icon.DORotate(new Vector3(0, 0, 0), 0.3f);
        }

        isOpen = !isOpen;
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(inputChat.text))
            return;

        player.SendMessage(inputChat.text);
        
        inputChat.text = String.Empty;
        inputChat.ActivateInputField();
    }
    
    public void PushMessage(string nickName, string msg, bool myChat)
    {
        if (!isOpen)
        {
            alarm.gameObject.SetActive(true);
            alarm.DOShakePosition(1f, 10);
        }

        StopAllCoroutines();

        var msgItem = Instantiate(txtChatItemBase, chatItemRoot);
        if(myChat) msgItem.color = Color.green;
        msgItem.text = $"[{nickName}] {msg}";
        msgItem.gameObject.SetActive(true);

        StartCoroutine(SetTextSize(msgItem));
        StartCoroutine(ScrollToBottom());
    }
    
    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        scroll.value = 0;
    }

    IEnumerator SetTextSize(TMP_Text textComp)
    {
        yield return new WaitForEndOfFrame();
        
        if(textComp.textInfo.lineCount > 1)
            textComp.rectTransform.sizeDelta = new Vector2(_baseChatItemWidth,textComp.preferredHeight + 12);
    }
}