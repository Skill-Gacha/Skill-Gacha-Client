using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using SRF;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    private static BossManager _instance = null;
    public static BossManager Instance => _instance;

    // pve 기준 검은 화면 출력 및 글자
    [SerializeField] private BossUIScreen bossUiScreen;

    // 배틀 로그 및 버튼(활성화/비활성화 포함) 정보 스크립트
    [SerializeField] private BossBattleLog uiBattleLog;

    // 내 UI 정보(속성, 이름, HP, MP) 스크립트
    [SerializeField] private BossUIPlayerInformation myInformation;

    public BossUIScreen BossUiScreen => bossUiScreen;
    public BossBattleLog UiBattleLog => uiBattleLog;
    public BossUIPlayerInformation MyInformation => myInformation;

    // 게임에 참여한 playerId 모음
    private int[] playersIds;

    // 게임에 참여한 유저 위치 모음
    [SerializeField] private Transform[] playerTrans;

    // 게임에 참여한 유저 애니메이터 모음
    private Animator[] playersAnimator = new Animator[3];

    // 게임에 참여한 팀원 HP, MP 창 모음
    [SerializeField] private BossUITeamInformation[] teamInformation;

    private Dictionary<int, string> monsterDb = new Dictionary<int, string>();

    [SerializeField] private Transform[] monsterSpawnPos;
    [SerializeField] private List<Monster> monsterObjs = new List<Monster>();

    private List<UIMonsterInformation> monsterUis = new List<UIMonsterInformation>();

    private string baseMonsterPath = "Monster/Monster1";

    // 해당 유저의 버튼 위치
    [SerializeField] private Transform buttons;

    private int[] animCodeList = new[]
    {
        Constants.PlayerBattleAttack1,
        Constants.PlayerBattleDie,
        Constants.PlayerBattleHit
    };

    private void Awake()
    {
        _instance = this;

        for (int i = 1; i <= 30; i++)
        {
            var monsterCode = Constants.MonsterCodeFactor + i;
            var monsterPath = $"Monster/Monster{i}";
            monsterDb.Add(monsterCode, monsterPath);
        }

        Set(GameManager.Instance.Boss);
        GameManager.Instance.Boss = null;
    }

    public void Set(S_BossMatchNotification Boss)
    {
        SetMonster(Boss.MonsterStatus);

        // Raid에 참여한 playerId 가져오기
        playersIds= Boss.PlayerIds.ToArray();
        // 파티에 참여한 모든 유저의 능력치 정보
        PlayerStatus[] playerStatus = Boss.PartyList.ToArray();
        for(int i = 0; i < playerStatus.Length; i++)
        {
            // 직업 Index 가져오기
            int classIdx = playerStatus[i].PlayerClass - Constants.PlayerCodeFactor;
            SetPlayer(classIdx, i);

            // 하단 체력, 마나, 본인 속성 띄워주는 창
            if(playersIds[i] == GameManager.Instance.PlayerId)
                MyInformation.Set(playerStatus[i]);

            // 팀 정보창에 정보 추가하기
            teamInformation[i].Set(playerStatus[i]);
        }

        if(Boss.BattleLog != null)
            // 본인의 배틀로그 적용
            uiBattleLog.Set(Boss.BattleLog);
    }

    public void SetMonster(MonsterStatus monster)
    {
        ResetMonster();

        var monsterCode = monster.MonsterModel;
        var monsterPath =  monsterDb.GetValueOrDefault(monsterCode, baseMonsterPath);
        var monsterRes = Resources.Load<Monster>(monsterPath);

        var dragon = Instantiate(monsterRes, monsterSpawnPos[0]);

        monsterObjs.Add(dragon);
        monsterUis.Add(dragon.UiMonsterInfo);

        dragon.UiMonsterInfo.SetName(monster.MonsterName);
        dragon.UiMonsterInfo.SetFullHp(monster.MonsterHp);


        BossMaterialChange(Random.Range(1001,1005));
        BossBarrierEnable();
    }

    // 나와 동료 직업에 따른 모델링과 애니메이션 가져오기
    private void SetPlayer(int classIdx, int index)
    {
        for(int i = 0; i < playerTrans[index].childCount; i++)
        {
            // 직업 index와 직업 모델링 index가 일치한다면
            bool select = classIdx == i;
            // 해당 직업 모델링 활성화 시키기
            GameObject character = playerTrans[index].GetChild(i).gameObject;
            character.SetActive(select);

            if(select)
                // 해당 유저 player의 애니메이터 가져오기
                playersAnimator[index] = character.GetComponent<Animator>();
        }
    }

    void ResetMonster()
    {
        for (var i = monsterObjs.Count - 1; i >= 0; i--)
        {
            if(monsterObjs[i] != null)
                Destroy(monsterObjs[i].gameObject);
        }

        monsterObjs.Clear();
        monsterUis.Clear();
    }

    public void SetMonster(RepeatedField<MonsterStatus> monsters)
    {
        ResetMonster();
        for (var i = 0; i < monsters.Count; i++)
        {
            var monsterInfo = monsters[i];
            var monsterCode = monsterInfo.MonsterModel;
            var monsterPath = monsterDb.GetValueOrDefault(monsterCode, baseMonsterPath);
            var monsterRes = Resources.Load<Monster>(monsterPath);
            var monster = Instantiate(monsterRes, monsterSpawnPos[i]);

            monsterObjs.Add(monster);
            monsterUis.Add(monster.UiMonsterInfo);

            monster.UiMonsterInfo.SetName(monsterInfo.MonsterName);
            monster.UiMonsterInfo.SetFullHp(monsterInfo.MonsterHp);
        }
    }

    public void SetMonsterHp(int idx, float hp)
    {
        if(idx < 0 || idx >= monsterUis.Count)
            return;
        monsterUis[idx].SetCurHp(hp);
    }


    public Monster GetMonster(int idx)
    {
        if (idx >= 0 || idx < monsterObjs.Count)
        {
            if(monsterObjs[idx] != null)
                return monsterObjs[idx];
        }

        return null;
    }

    public List<Monster> GetMonster(int[] monsterIndex)
    {
        return monsterIndex.Where(index => index >= 0 && index < monsterObjs.Count).Select(index => monsterObjs[index]).ToList();
    }

    public void PlayerHit(int[] playerId)
    {
        for(int i = 0; i < playerId.Count(); i++)
        {
            TriggerAnim(playerId[i],Constants.PlayerBattleHit);
        }
    }

    private void TriggerAnim(int playerId,int code)
    {
        for(int i = 0; i < playersIds.Count();i++)
        {
            if(playersIds[i] == playerId)
            {
                playersAnimator[i].transform.localEulerAngles = Vector3.zero;
                playersAnimator[i].transform.localPosition = Vector3.zero;
                playersAnimator[i].applyRootMotion = code == Constants.PlayerBattleDie;
                playersAnimator[i].SetTrigger(code);
            }
        }
    }

    // playerId에 해당하는 index를 반환하는 함수
    public int GetPlayerIndexById(int playerId)
    {
        for(int i = 0; i < playersIds.Count();i++)
        {
            if(playersIds[i] == playerId) return i;
        }
        // 해당 id가 없을 경우
        return -1;
    }

    public int[] GetPlayersIndex()
    {
        return playersIds;
    }

    public void PlayerAnim(int playerId,int idx)
    {
        if(idx < 0 || idx >= animCodeList.Length)
            return;

        var animCode = animCodeList[idx];
        TriggerAnim(playerId,animCode);
    }

    public void SetPartyStatus(int playerId, int Hp, int Mp)
    {
        int playerIdx = GetPlayerIndexById(playerId);

        float playerCurHp = Hp;
        float playerCurMp = Mp;

        teamInformation[playerIdx].SetCurHp(playerCurHp);
        teamInformation[playerIdx].SetCurMp(playerCurMp);

        // 본인 Status 창 업데이트
        if (playerId == GameManager.Instance.PlayerId)
        {
            MyInformation.SetCurHp(playerCurHp);
            MyInformation.SetCurMp(playerCurMp);
        }
    }

    public void BossMaterialChange(int randomElement)
    {
        int elementIndex = randomElement - Constants.PlayerCodeFactor;
        //1001 : 전기 => 0
        //1002 : 땅 => 1
        //1003 : 풀 => 2
        //1004 : 물 => 3
        //1005 : 불 => 4
        Debug.Log("randomElement : "+randomElement);
        Debug.Log("elementIndex : "+elementIndex);
        BossScript bossScript = monsterSpawnPos[0].GetChild(0).GetComponent<BossScript>();
        bossScript.SetMaterial(elementIndex);
    }

    public void BossBarrierEnable()
    {
        UIMonsterInformation monsterInfo = monsterSpawnPos[0].GetComponentInChildren<UIMonsterInformation>();
        Debug.Log("monsterInfo : "+monsterInfo);
        monsterInfo.EnableBarrierImage();
    }

    public void BossBarrierBreak(int count)
    {
        UIMonsterInformation monsterInfo = monsterSpawnPos[0].GetComponentInChildren<UIMonsterInformation>();
        Debug.Log("monsterInfo : "+monsterInfo);
        monsterInfo.BreakBarrierImage(count);
    }
}