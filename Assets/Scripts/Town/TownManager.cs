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
        Vector3 eRot = new Vector3(0, tr.Rot, 0);
        var spawnRot = Quaternion.Euler(eRot);

        var playerId = playerInfo.PlayerId;
        var playerResPath = playerDb.GetValueOrDefault(playerInfo.Class, basePlayerPath);
        var playerRes = Resources.Load<Player>(playerResPath);
        var player = Instantiate(playerRes, spawnPos, spawnRot);
        player.Move(spawnPos, spawnRot);
        player.SetPlayerId(playerId);
        player.SetNickname(playerInfo.Nickname);

        if (playerList.ContainsKey(playerId))
        {
            var prevPlayer = playerList[playerId]; 
            playerList[playerId] = player;
            
            if(prevPlayer)
                Destroy(prevPlayer.gameObject);
        }
        else
        {
            playerList.Add(playerId, player);   
        }

        return player;
    }

    public void ReleasePlayer(int playerId)
    {
        if (!playerList.ContainsKey(playerId)) 
            return;

        var player = playerList[playerId];
        playerList.Remove(playerId);
        
        Destroy(player.gameObject);
    }

    public void TurnGameUI()
    {
        uiStart.gameObject.SetActive(false);
        
        uiChat.gameObject.SetActive(true);
        uiAnimation.gameObject.SetActive(true);
    }
    
    public Player GetPlayerAvatarById(int playerId)
    {
        if (playerList.ContainsKey(playerId))
            return playerList[playerId];
        
        return null;
    }
}
