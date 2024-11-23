using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
        // Player 클래스에서 '나'인지 판단하는 변수에 진위여부를 담고
        if (IsMine)
        {
            // 내가 맞으면 해당 gameObject에 MyPlayer 스크립트(C# 코드를) 붙여줍니다.
            mPlayer = gameObject.AddComponent<MyPlayer>();

        }
        else
            // 아니면, 오류 상황!!! 해당 케리터는 유저 소유의 케릭터가 아니므로
            // 케릭터를 멋대로 조작할 수 없게 이동 관련 컴포넌트를 제거합니다.
            Destroy(gameObject.GetComponent<NavMeshAgent>());

        uiChat = TownManager.Instance.UiChat;
        // 마을의 채팅창을 유저에게도 제공해줍니다.

        isInit = true;
        // 아직 사용 이유 모름
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
        // 본인이 가진 채팅창에 msg를 넣는다.
        uiChat.PushMessage(nickname, msg, IsMine);
    }

    public void Move(Vector3 move, Quaternion rot)
    {
        goalPos = move;
        // 이동할 목표 위치에 move값 주기
        goalRot = rot;
        // 이동할 목표 위치까지 회전 값
    }

    public void Animation(int animCode)
    {
        if(animator)
        //애니메이션 콤포넌트가 있나 확인
            animator.SetTrigger(animCode);
            // 해당 에니메이션을 실행해라는 Unity 명령어
    }

    void CheckMove()
    {
        float dist = Vector3.Distance(lastPos, transform.position);
        animator.SetFloat(Constants.TownPlayerMove, dist * 100);


        lastPos = transform.position;
    }
}
