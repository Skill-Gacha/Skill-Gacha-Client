using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    [SerializeField] private UINameChat uiNameChat;

    public Avatar avatar { get; private set; }
    public MyPlayer mPlayer { get; private set; }
    
    private string nickname;

    private UIChat uiChat;
    
    private Vector3 goalPos;
    private Quaternion goalRot;

    private Animator animator;

    public int PlayerId { get; private set; }
    public bool IsMine  { get; private set; }
    private bool isInit = false;

    private Vector3 lastPos;

    void Start()
    {
        avatar = GetComponent<Avatar>();
        animator = GetComponent<Animator>();
    }

    public void SetPlayerId(int playerId)
    {
        PlayerId = playerId;
    }

    public void SetNickname(string nickname)
    {
        this.nickname = nickname;
        uiNameChat.SetName(nickname);
    }

    public void SetIsMine(bool isMine)
    {
        IsMine = isMine;

        if (IsMine)
        {
            mPlayer = gameObject.AddComponent<MyPlayer>();
        }
        else
            Destroy(gameObject.GetComponent<NavMeshAgent>());

        uiChat = TownManager.Instance.UiChat;
        isInit = true;
    }


    private void Update()
    {
        if(isInit == false)
            return;
        
        if (IsMine)
            return;

        if (Vector3.Distance(transform.position, goalPos) > 0.5f)
            transform.position = goalPos;
        else
            transform.position = Vector3.Lerp(transform.position, goalPos, Time.deltaTime * 10);

        if (goalRot != Quaternion.identity)
        {
            float t = Mathf.Clamp(Time.deltaTime * 10, 0, 0.99f);
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRot, t);
        }

        CheckMove();
    }

    public void SendMessage(string msg)
    {
        if(!IsMine) return;

        C_Chat chatPacket = new C_Chat
        {
            PlayerId = PlayerId,
            SenderName = nickname, 
            ChatMsg = msg
        };
        
        GameManager.Network.Send(chatPacket);
    }
    
    public void RecvMessage(string msg)
    {
        uiNameChat.PushText(msg);
        uiChat.PushMessage(nickname, msg, IsMine);
    }

    public void Move(Vector3 move, Quaternion rot)
    {
        goalPos = move;
        goalRot = rot;
    }
    
    public void Animation(int animCode)
    {
        if(animator)
            animator.SetTrigger(animCode);
    }
    
    void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);


        lastPos = transform.position;
    }
}
