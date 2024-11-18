using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    [SerializeField] private GameObject charList;
    [SerializeField] private Button[] charBtns;
    
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnBack;
    [SerializeField] private TMP_InputField inputNickname;
    [SerializeField] private TMP_InputField inputPort;
    [SerializeField] private TMP_Text txtMessage;
    private TMP_Text placeHolder;

    private int classIdx = 0;

    private string serverUrl;
    private string nickname;
    private string port;
    
    void Start()
    {
        placeHolder = inputNickname.placeholder.GetComponent<TMP_Text>();
        btnBack.onClick.AddListener(SetServerUI);
        
        SetServerUI();
        
        for (int i = 0; i < charBtns.Length; i++)
        {
            int idx = i;
            charBtns[i].onClick.AddListener(() =>
            {
                SelectCharacter(idx);
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (inputNickname.IsActive())
                btnConfirm.onClick.Invoke();
        }
    }

    void SelectCharacter(int idx)
    {
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(false);
        
        classIdx = idx;
        
        charBtns[classIdx].transform.GetChild(0).gameObject.SetActive(true);
    }

    void SetServerUI()
    {
        txtMessage.color = Color.white;
        txtMessage.text = "Welcome!";

        inputNickname.text = string.Empty;
        placeHolder.text = "서버주소를 입력해주세요!";
        
        charList.gameObject.SetActive(false);
        btnBack.gameObject.SetActive(false);
        inputPort.gameObject.SetActive(true);
        
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmServer);
    }

    void SetNicknameUI()
    {
        txtMessage.color = Color.white;
        txtMessage.text = "Welcome!";

        inputNickname.text = string.Empty;
        placeHolder.text = "닉네임을 입력해주세요 (2~10글자)";
        
        charList.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(true);
        inputPort.gameObject.SetActive(false);
        
        btnConfirm.onClick.RemoveAllListeners();
        btnConfirm.onClick.AddListener(ConfirmNickname);
    }

    void ConfirmServer()
    {
        
        txtMessage.color = Color.red;
        // if (string.IsNullOrEmpty(inputNickname.text))
        // {
        //     txtMessage.text = "서버주소를 입력해주세요!!";
        //     return;
        // }

        serverUrl = inputNickname.text;
        port = inputPort.text;
        SetNicknameUI();
    }
    
    void ConfirmNickname()
    {
        txtMessage.color = Color.red;
        
        if (inputNickname.text.Length < 2)
        {
            txtMessage.text = "이름을 2글자 이상 입력해주세요!";
            return;
        }

        if (inputNickname.text.Length > 10)
        {
            txtMessage.text = "이름을 10글자 이하로 입력해주세요!";
            return;
        }

        nickname = inputNickname.text;
        
        TownManager.Instance.GameStart(serverUrl, port, nickname, classIdx);
        gameObject.SetActive(false);
    }
}
