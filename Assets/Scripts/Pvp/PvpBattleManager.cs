using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class PvpBattleManager : MonoBehaviour
{
    private static PvpBattleManager _instance = null;

    // 싱글톤 방식으로 본인 객체 참조
    public static PvpBattleManager Instance => _instance;

    [SerializeField] private UIScreen uiScreen;
    [SerializeField] private UIBattleLog uiBattleLog;
    [SerializeField] private PvpUIPlayerInformation uiPlayerInformation;

    public UIScreen UiScreen => uiScreen;
    public UIBattleLog UiBattleLog => uiBattleLog;

    public PvpUIPlayerInformation UiPlayerInformation => uiPlayerInformation;

    // pvp 맵 환경
    [SerializeField] private Maps map;

    // 플레이어(나)의 위치 정보를 가진 배열 playerTrans
    [SerializeField] private Transform[] playerTrans;

    // 플레이어(나)의 Animator
    private Animator playerAnimator;


    // 상대방이 생성 될 위치
    [SerializeField] private Transform[] opponentTrans;

    // 상대방의 Animator
    private Animator opponentAnimator;

    // 상대방 정보를 모든 담은 변수
    [SerializeField] private Player opponentObjs;

    // 상대방 정보를 출력해줄 UI
    [SerializeField] PvpUIOpponentInformation uiOpponentInformation;

    // 외부에서 사용할 수 있게 private 변수를 얕은 복사한 대상
    public PvpUIOpponentInformation UIOpponentInformation => uiOpponentInformation;


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
        // if (pvp.DungeonInfo != null)
        //     SetDungeon(pvp.DungeonInfo);

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

        // 버튼 선택 가능 여부는 게임 시작 시 지급을 안 하는 중이라 주석 처리된 상태
        // 만약 필요하다면 주석 해제
        // if (pvp.BattleLog != null)
        //     uiBattleLog.Set(pvp.BattleLog);
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
                animator = trans[i].GetComponent<Animator>();
        }

        if (isPlayer)
            playerAnimator = animator;
        else
            opponentAnimator = animator;
    }

    // 던전 배경 설정 현재는 호출부가 주석 처리된 상태
    public void SetDungeon(DungeonInfo dungeonInfo)
    {
        SetMap(dungeonInfo.DungeonCode);
    }

    // 플레이어가 공격하겠다는 함수
    public void PlayerHit(bool isPlayer)
    {
        Animator actor = isPlayer ? playerAnimator : opponentAnimator;
        Animator stopper = !isPlayer ? opponentAnimator : playerAnimator;

        TriggerAnim(actor, stopper,Constants.PlayerBattleHit);
    }

    // 애니메이션이 유효한지 판단 후 animCodeList배열에서
    // idx에 맞는 애니메이션 코드 가져오기
    public void PlayerAnim(int idx)
    {
        if(idx < 0 || idx >= animCodeList.Length)
            return;

        var animCode = animCodeList[idx];
        TriggerAnim(animCode);
    }

    // 애니메이션 동작하게 만들기
    void TriggerAnim(Animator actor, Animator stopper, int code)
    {
        playerAnimator.transform.localEulerAngles = Vector3.zero;
        playerAnimator.transform.localPosition = Vector3.zero;
        playerAnimator.applyRootMotion = code == Constants.PlayerBattleDie;
        playerAnimator.SetTrigger(code);
    }

    // 맵 설정
    public void SetMap(int id)
    {
        map.SetMap(id);
    }
}
