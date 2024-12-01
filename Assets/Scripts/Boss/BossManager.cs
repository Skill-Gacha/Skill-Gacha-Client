using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using SRF;
using Unity.VisualScripting;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    private static BossManager _instance = null;
    public static BossManager Instance => _instance;

    [SerializeField] private BossUIScreen uiScreen;
    [SerializeField] private BossBattleLog uiBattleLog;
    [SerializeField] private BossUIPlayerInformation myInformation;

    public BossUIScreen UiScreen => uiScreen;
    public BossBattleLog UiBattleLog => uiBattleLog;
    public BossUIPlayerInformation MyInformation => myInformation;

    private int[] playersIds;

    [SerializeField] private Transform[] playerTrans;

    private Animator[] playersAnimator = new Animator[3];

    private Dictionary<int, string> monsterDb = new Dictionary<int, string>();

    [SerializeField] private Transform[] monsterSpawnPos;
    [SerializeField] private List<Monster> monsterObjs = new List<Monster>();

    private List<UIMonsterInformation> monsterUis = new List<UIMonsterInformation>();

    private string baseMonsterPath = "Monster/Monster1";


    private int[] animCodeList = new[]
    {
        Constants.PlayerBattleAttack1,
        Constants.PlayerBattleDie,
        Constants.PlayerBattleHit
    };



    private void Awake()
    {
        _instance = this;

        for (int i = 1; i < 30; i++)
        {
            var monsterCode = Constants.MonsterCodeFactor + i;
            var monsterPath = $"Monster/Monster{i}";
            monsterDb.Add(monsterCode, monsterPath);
        }

        Set(GameManager.Instance.Boss);
        GameManager.Instance.Boss = null;
    }

    public void Set(S_BossMatchNotification Raid)
    {
        // Raid에 참여한 playerId 가져오기
        playersIds= Raid.PlayerIds.ToArray();
        // 파티에 참여한 모든 유저의 능력치 정보
        PlayerStatus[] playerStatus = Raid.PartyList.ToArray();
        for(int i = 0; i < playerStatus.Length; i++)
        {
            // 직업 Index 가져오기
            int classIdx = playerStatus[i].PlayerClass - Constants.PlayerCodeFactor;
            SetPlayer(classIdx, i);

            // 하단 체력, 마나, 본인 속성 띄워주는 창
            if(playersIds[i] == GameManager.Instance.PlayerId)
                MyInformation.Set(playerStatus[i]);
        }

        if(Raid.BattleLog != null)
            // 본인의 배틀로그 적용
            uiBattleLog.Set(Raid.BattleLog);
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
            monster.UiMonsterInfo.SetFullHP(monsterInfo.MonsterHp);
        }
    }

    public void SetMonsterHp(int idx, float hp)
    {
        if(idx < 0 || idx >= monsterUis.Count)
            return;
        monsterUis[idx].SetCurHP(hp);
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

    public void PlayerHit()
    {
        TriggerAnim(Constants.PlayerBattleHit);
    }

    void TriggerAnim(int code)
    {
        //playerAnimator.transform.localEulerAngles = Vector3.zero;
        //playerAnimator.transform.localPosition = Vector3.zero;
        //playerAnimator.applyRootMotion = code == Constants.PlayerBattleDie;
        //playerAnimator.SetTrigger(code);
    }

    public void PlayerAnim(int idx)
    {
        if(idx < 0 || idx >= animCodeList.Length)
            return;

        var animCode = animCodeList[idx];
        TriggerAnim(animCode);
    }
}
