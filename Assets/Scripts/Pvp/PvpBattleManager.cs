using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class PvpBattleManager : MonoBehaviour
{
    private static PvpBattleManager _instance = null;

    // 싱글톤 방식으로 본인 객체 참조
    public static PvpBattleManager Instance => _instance;

    [SerializeField] private PvpUIScreen pvpUiScreen;
    [SerializeField] private PvpBattleLog uiBattleLog;
    [SerializeField] private PvpUIPlayerInformation uiPlayerInformation;

    public PvpUIScreen PvpUiScreen => pvpUiScreen;
    public PvpBattleLog PvpUiBattleLog => uiBattleLog;

    public PvpUIPlayerInformation UiPlayerInformation => uiPlayerInformation;

    [SerializeField] private Transform buttons;

    // pvp 맵 환경
    [SerializeField] private Maps map;

    // 플레이어(나)의 위치 정보를 가진 배열 playerTrans
    [SerializeField] private Transform[] playerTrans;

    // 플레이어(나)의 Animator
    private Animator playerAnimator;

    private Player myPlayer;

    // 상대방이 생성 될 위치
    [SerializeField] private Transform[] opponentTrans;

    // 상대방의 Animator
    private Animator opponentAnimator;

    // 상대방 정보를 모든 담은 변수
    [HideInInspector] public Player opponentPlayer = null;

    // 상대방 정보를 출력해줄 UI
    [SerializeField] private PvpUIOpponentInformation uiOpponentInformation;

    // 외부에서 사용할 수 있게 private 변수를 얕은 복사한 대상
    public PvpUIOpponentInformation UIOpponentInformation => uiOpponentInformation;

    // 누구의 유저 턴인지 구분하는 bool
    bool CheckWhoTurn;
    // true이면 서버의 matching 당시 playerA의 공격 차례
    // false이면 서버의 matching 다시 playerB의 공격 차례

    // 플레이어(나) 공격, 죽기, 때리기 모션을 hash(인트형) 코드
    private int[] animCodeList = new[]
    {
        Constants.PlayerBattleAttack1,
        Constants.PlayerBattleDie,
        Constants.PlayerBattleHit
    };

    private void Awake()
    {
        _instance = this;

        // 초기 세팅 시작
        Set(GameManager.Instance.Pvp);
        // Pvp 게임 환경 조성 하는 함수

        // 게임 매니저로 받은 게임 정보 초기화
        // 다음 게임 정보를 받기 위해서 사용
        GameManager.Instance.Pvp = null;
    }

    public void Set(S_PlayerMatchNotification pvp)
    {
        // 현재는 패킷으로 보내는 부분이 없어서 문제 발생 중
        // if (pvp.dungeonCode != null)
        //     SetDungeon(pvp.dungeonCode);

        // 내 캐릭터 설정
        if (pvp.PlayerData != null)
        {
            // 내 캐릭터 UI 정보 설정
            uiPlayerInformation.Set(pvp.PlayerData);
            // 내 캐릭터 3D 모델 설정
            SetCharacter(pvp.PlayerData.PlayerClass, true);
        }

        // 상대방 캐릭터 설정
        if (pvp.OpponentData != null)
        {
            // 상대방 캐릭터 UI 정보 설정
            uiOpponentInformation.Set(pvp.OpponentData);
            // 상대방 캐릭터 3D 모델 설정
            SetCharacter(pvp.OpponentData.PlayerClass, false);
        }
        Debug.Log(pvp.BattleLog);

        if (pvp.BattleLog != null)
            uiBattleLog.Set(pvp.BattleLog);
    }

    // 캐릭터 모델 설정 및 애니메이터 할당
    private void SetCharacter(int classCode, bool isPlayer)
    {
        int idx = classCode - Constants.PlayerCodeFactor;
        Transform[] trans = isPlayer ? playerTrans : opponentTrans;
        Animator animator = null;

        for (int i = 0; i < trans.Length; i++)
        {
            bool select = i == idx;
            trans[i].gameObject.SetActive(select);

            if (select)
            {
                animator = trans[i].GetComponent<Animator>();
                if(isPlayer) myPlayer = trans[i].GetComponent<Player>();
                else opponentPlayer = trans[i].GetComponent<Player>();
            }
        }

        if (isPlayer)
            playerAnimator = animator;
        else
            opponentAnimator = animator;
    }

    // 던전 배경 설정 현재는 호출부가 주석 처리된 상태
    public void SetDungeon(int dungeonCode)
    {
        SetMap(dungeonCode);
    }

    // 맵 설정
    public void SetMap(int id)
    {
        map.SetMap(id);
    }

    public void CheckUserTurn(bool UserTurn)
    {
        Debug.Log("동작 유무 확인"+UserTurn);
        int numChildren = buttons.transform.childCount;
        for(int i = 0; i < numChildren; i++)
        {
            Button button = buttons.GetChild(i).GetComponent<Button>();
            button.interactable = UserTurn;
        }
    }

    public void PlayerHit(bool isMyPlayer)
    {
        TriggerPlayerAction(Constants.PlayerBattleHit, isMyPlayer);
    }

    public void PlayerAnim(int idx, bool isMyPlayer)
    {
        if (idx < 0 || idx >= animCodeList.Length) return;

        int animCode = animCodeList[idx];
        TriggerPlayerAction(animCode, isMyPlayer);
    }

    private void TriggerPlayerAction(int code, bool isMyPlayer)
    {
        Animator animator = isMyPlayer ? playerAnimator : opponentAnimator;

        animator.transform.localEulerAngles = isMyPlayer ? Vector3.zero : new Vector3(0,180,0);
        animator.transform.localPosition = Vector3.zero;
        animator.applyRootMotion = code == Constants.PlayerBattleDie;
        animator.SetTrigger(code);
    }
}
