using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class TownManager : MonoBehaviour
{
    private static TownManager _instance = null;
    public static TownManager Instance => _instance;

    [SerializeField] private CinemachineFreeLook freeLook;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private EventSystem eSystem;
    public CinemachineFreeLook FreeLook => freeLook;
    public EventSystem E_System => eSystem;

    public Player myPlayer { get; private set; }

    [SerializeField] private UIStart uiStart;
    [SerializeField] private UIAnimation uiAnimation;
    [SerializeField] private UIChat uiChat;

    [SerializeField] private UIStore uiStore;
    [SerializeField] private UIEnhance uiEnhance;

    [SerializeField] private UIMatching uIMatching;
    [SerializeField] private UIBossMatching uIBossMatching;
    [SerializeField] private UIBossMatchingFail uIBossMatchingFail;

    public UIMatching UIMatching => uIMatching;

    public UIBossMatching UIBossMatching => uIBossMatching;
    public UIBossMatchingFail UIBossMatchingFail => uIBossMatchingFail;

    public UIStore UIStore => uiStore;
    public UIEnhance UIEnhance => uiEnhance;

    [SerializeField] private UIRank uIRank;

    public UIRank UIRank => uIRank;

    [SerializeField] private UIInventory uiInventory;

    public UIInventory UIInventory => uiInventory;

    public UIChat UiChat => uiChat;

    [SerializeField] private TMP_Text txtServer;


    private Dictionary<int, Player> playerList = new Dictionary<int, Player>();
    private Dictionary<int, string> playerDb = new Dictionary<int, string>();

    private string basePlayerPath = "Player/Player1";

    private void Awake()
    {
        _instance = this;
        playerDb.Add(1001, "Player/Player1");
        playerDb.Add(1002, "Player/Player2");
        playerDb.Add(1003, "Player/Player3");
        playerDb.Add(1004, "Player/Player4");
        playerDb.Add(1005, "Player/Player5");
    }

    private void Start()
    {
        if (GameManager.Network.IsConnected == false)
        {
            uiStart.gameObject.SetActive(true);
        }
        else
        {
            Connected();
        }
    }

    public void GameStart(string gameServer, string port, string userName, int classIdx)
    {
        GameManager.Network.Init(gameServer, port);
        GameManager.Instance.UserName = userName;
        GameManager.Instance.ClassIdx = classIdx + 1001;

        txtServer.text = gameServer;
    }

    public void Connected()
    {
        C_Enter enterPacket = new C_Enter
        {
            Nickname = GameManager.Instance.UserName,
            Class = GameManager.Instance.ClassIdx
        };
        GameManager.Network.Send(enterPacket);
    }

    public void Spawn(PlayerInfo playerInfo)
    {
		GameManager.Instance.SetPlayerId(playerInfo.PlayerId);
        var tr = playerInfo.Transform;
        var spawnPos = spawnArea.position;
        spawnPos.x += tr.PosX;
        spawnPos.z += tr.PosZ;
        myPlayer = CreatePlayer(playerInfo, spawnPos);
        myPlayer.SetIsMine(true);

        TurnGameUI();
    }

    public Player CreatePlayer(PlayerInfo playerInfo, Vector3 spawnPos)
    {
        var tr = playerInfo.Transform;
        // 유저 정보에서 위치 정보를 가져와

        Vector3 eRot = new Vector3(0, tr.Rot, 0);
        var spawnRot = Quaternion.Euler(eRot);
        // 회전 상태 구하기(Quaternion.Euler은 짐벌락 문제를 해결하기 위해 사용)

        var playerId = playerInfo.PlayerId;
        // 아이디를 변수에 담기

        var playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, basePlayerPath);
        // playerDb는 직업에 따른 "Resources/Player/~~~~~"의 프리팹 모델 경로를 가져옵니다.

        var playerRes = Resources.Load<Player>(playerResPath);
        // 그 후, 그 경로의 프리팹을 가져옵니다.

        var player = Instantiate(playerRes, spawnPos, spawnRot);
        // Instantiate는 동적으로 GameObject를 만드는 유니티 함수로
        // 첫 번째 인자는 적용시킬 프리팹 또는 2, 3모델
        // 두 번째 인자는 GameObject를 생성시킬 좌표
        // 세 번째 인자는 GameObject의 회전 상태
        // 가 필요합니다.
        // (함수 오버로딩으로 인자가 몇 개 없거나 다른 값이 들어와도 함수가 동작할 수 있습니다.)
        // Instantiate 함수 개수가 보고 싶다면 해당 함수를 Control + 클릭 하기

        player.Move(spawnPos, spawnRot);
        // 좌표와 회전 상태로 움직임에 사용되는 함수지만,
        // 지금은 리스폰에 사용되고 있습니다.

        player.SetPlayerId(playerId);
        // player Class에 playerId를 변수에 저장할 수 있게 제공

        player.SetNickname(playerInfo.Nickname);
        // 닉네임도 제공될 수 있게 제공

        // id는 보안이 필요한 정보라 없지만
        // 닉네임은 해당 케릭터 머리 위에 표시 돼야 하기 때문에
        // ~~~.text = nickname으로 사용하는 부분이 있을 것 입니다.


        if (playerList.ContainsKey(playerId))
        {
            // Town 마을에 있는 플레이어 리스트(딕셔너리, Map)에서 해당 유저의 Key가 존재한다면
            // 오류 상황! 나는 던전이나 PVP로 이동했어서 마을에 없어야 했음
            var prevPlayer = playerList[playerId];
            playerList[playerId] = player;
            if (prevPlayer)
            Destroy(prevPlayer.gameObject);
            // 기존 유저 정보와 케릭터를 제거하고, 새롭게 넣어줍니다.

            // Destroy는 Unity에서 GameObject를 제거해주는 함수 입니다.
        }
        else
        {
            // 정상적인 상황이었다면,
            playerList.Add(playerId, player);
            // 유저 id를 key로 GameObject를 value로
            // 마을의 유저 정보에 추가해줍니다.
        }

        return player;
        // 새롭게 생성된 GameObject를 반환해줍니다.
        // 해당 게임 오브젝트에는 유저의 모든 정보와 3모델이 존재합니다.
        // 헷갈리면 위로 올라가서 어떻게 player가 생성되는지 보세요
    }

    public void ReleasePlayer(int playerId)
    // 마을에서 사라지게 할 플레이어
    {
        if (!playerList.ContainsKey(playerId))
            return;
        // 존재하지 않으면 그냥 종료 처리

        var player = playerList[playerId];
        // playerList(Dictionary, map) 자료형 입니다.
        // playerList에 있는 playerId(key)에 맞는 player(value)가 나오게 해줍니다.
        // player는 해당 유저(케릭터)의 모든 정보를 가지고 있는 대상 입니다.

        playerList.Remove(playerId);
        // playerList에서 playerId(key)에 따른 value를 제거한다.

        Destroy(player.gameObject);
        // player의 게임 오브젝트를 삭제한다.
    }

    public void TurnGameUI()
    {
        // 게임 시작화면(주소, 포트 입력 및 닉네임 작성 UI)
        uiStart.gameObject.SetActive(false);
        // 체팅 UI 활성화
        uiChat.gameObject.SetActive(true);

        // 유저가 선택 가능한 감정표현 UI 활성화
        uiAnimation.gameObject.SetActive(true);
    }

    public void Pvp()// 나중에() 지우고 inspecter 창에서 매칭 잡을 때는 체크 아닐 때는,  체크 해제하기(bool PvpCheck)
    {
        //매칭 요청
        C_PlayerMatch response = new C_PlayerMatch(); // 나중에 () 지우고 {  PvpCheck };
        GameManager.Network.Send(response);
    }

    public void BossMatch()
    {
        //매칭 요청
        C_BossMatch response = new C_BossMatch() { };
        GameManager.Network.Send(response);
    }

    public void BossMatchAccept()
    {
        //매칭 요청
        C_AcceptResponse response = new C_AcceptResponse() { };
        GameManager.Network.Send(response);
    }

    public Player GetPlayerAvatarById(int playerId)
    {
        if (playerList.ContainsKey(playerId))
            // playerList(마을 안에 있는 유저 정보)(딕셔너리, Map)에 playerId가 포함 돼 있으면
            return playerList[playerId];
        // playerId(key)에 따른 value인 유저 정보를 반환해준다.
        return null;
    }
}
