// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossManager.cs 리팩터링 및 오류 수정 -----

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    private static BossManager _instance = null;
    public static BossManager Instance => _instance;

    [SerializeField] private BossUIScreen bossUiScreen;
    [SerializeField] private BossBattleLog uiBattleLog;
    [SerializeField] private BossUIPlayerInformation myInformation;
    [SerializeField] private Transform[] playerTrans;
    [SerializeField] private BossUITeamInformation[] teamInformation;
    [SerializeField] private Transform[] monsterSpawnPos;

    public BossUIScreen BossUiScreen => bossUiScreen;
    public BossBattleLog UiBattleLog => uiBattleLog;
    public BossUIPlayerInformation MyInformation => myInformation;

    private int[] playersIds;
    private Animator[] playersAnimator = new Animator[3];
    private Dictionary<int, string> monsterDb = new Dictionary<int, string>();
    private List<Monster> monsterObjs = new List<Monster>();
    private List<UIMonsterInformation> monsterUis = new List<UIMonsterInformation>();
    private string baseMonsterPath = "Monster/Monster1";

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
        InitializeMonsterDatabase();
        InitializeBoss();
    }

    private void InitializeMonsterDatabase()
    {
        for (int i = 1; i <= 30; i++)
        {
            int monsterCode = Constants.MonsterCodeFactor + i;
            string monsterPath = $"Monster/Monster{i}";
            monsterDb.Add(monsterCode, monsterPath);
        }
    }

    private void InitializeBoss()
    {
        if (GameManager.Instance.Boss != null)
        {
            Set(GameManager.Instance.Boss);
            GameManager.Instance.Boss = null;
        }
    }

    public void Set(S_BossMatchNotification bossNotification)
    {
        if (bossNotification == null) return;

        // 단일 MonsterStatus인 경우
        if (bossNotification.MonsterStatus != null)
        {
            SetMonsters(bossNotification.MonsterStatus);
        }

        // 플레이어 ID는 RepeatedField<int>이므로 ToArray()를 사용하여 int[]로 변환
        SetPlayers(bossNotification.PlayerIds.ToArray(), bossNotification.PartyList);
        SetBattleLog(bossNotification.BattleLog);
    }

    // 단일 MonsterStatus를 처리하는 SetMonsters 메서드
    private void SetMonsters(MonsterStatus singleMonster)
    {
        if (singleMonster == null) return;

        ResetMonsters();
        SpawnMonster(singleMonster);
    }

    private void SpawnMonster(MonsterStatus monsterStatus)
    {
        string monsterPath = monsterDb.GetValueOrDefault(monsterStatus.MonsterModel, baseMonsterPath);
        Monster monsterPrefab = Resources.Load<Monster>(monsterPath);
        if (monsterPrefab == null)
        {
            Debug.LogWarning($"Monster prefab not found at path: {monsterPath}");
            return;
        }

        // 몬스터를 스폰할 위치 인덱스 계산
        int spawnIndex = monsterObjs.Count < monsterSpawnPos.Length ? monsterObjs.Count : 0;
        Transform spawnTransform = monsterSpawnPos[spawnIndex];
        Monster monsterInstance = Instantiate(monsterPrefab, spawnTransform);

        monsterObjs.Add(monsterInstance);
        monsterUis.Add(monsterInstance.UiMonsterInfo);

        monsterInstance.UiMonsterInfo.SetName(monsterStatus.MonsterName);
        monsterInstance.UiMonsterInfo.SetFullHp(monsterStatus.MonsterHp);
    }

    private void ResetMonsters()
    {
        foreach (var monster in monsterObjs)
        {
            if (monster != null)
                Destroy(monster.gameObject);
        }
        monsterObjs.Clear();
        monsterUis.Clear();
    }

    private void SetPlayers(int[] playerIds, RepeatedField<PlayerStatus> partyList)
    {
        playersIds = playerIds;
        PlayerStatus[] playerStatuses = partyList.ToArray();

        for (int i = 0; i < playerStatuses.Length; i++)
        {
            int classIdx = playerStatuses[i].PlayerClass - Constants.PlayerCodeFactor;
            SetPlayerModel(classIdx, i);
            SetPlayerInformation(playerStatuses[i], i);
        }
    }

    private void SetPlayerModel(int classIdx, int index)
    {
        for (int i = 0; i < playerTrans[index].childCount; i++)
        {
            bool isSelected = classIdx == i;
            GameObject character = playerTrans[index].GetChild(i).gameObject;
            character.SetActive(isSelected);

            if (isSelected)
                playersAnimator[index] = character.GetComponent<Animator>();
        }
    }

    private void SetPlayerInformation(PlayerStatus playerStatus, int index)
    {
        if (playersIds[index] == GameManager.Instance.PlayerId)
            MyInformation.Set(playerStatus);

        teamInformation[index].Set(playerStatus);
    }

    private void SetBattleLog(BattleLog battleLog)
    {
        if (battleLog != null)
            uiBattleLog.Set(battleLog);
    }

    public void SetMonsterHp(int idx, float hp)
    {
        if (IsValidMonsterIndex(idx))
            monsterUis[idx].SetCurHp(hp);
    }

    private bool IsValidMonsterIndex(int idx)
    {
        return idx >= 0 && idx < monsterUis.Count;
    }

    public Monster GetMonster(int idx)
    {
        if (idx >= 0 && idx < monsterObjs.Count)
            return monsterObjs[idx];
        return null;
    }

    public List<Monster> GetMonster(int[] monsterIndices)
    {
        return monsterIndices.Where(index => index >= 0 && index < monsterObjs.Count)
                             .Select(index => monsterObjs[index])
                             .ToList();
    }

    public void PlayerHit(int[] playerIds)
    {
        foreach (var playerId in playerIds)
        {
            TriggerAnimation(playerId, Constants.PlayerBattleHit);
        }
    }

    private void TriggerAnimation(int playerId, int code)
    {
        int playerIndex = GetPlayerIndexById(playerId);
        if (playerIndex == -1) return;

        Animator animator = playersAnimator[playerIndex];
        if (animator == null) return;

        animator.transform.localEulerAngles = Vector3.zero;
        animator.transform.localPosition = Vector3.zero;
        animator.applyRootMotion = code == Constants.PlayerBattleDie;
        animator.SetTrigger(code);
    }

    public int GetPlayerIndexById(int playerId)
    {
        for (int i = 0; i < playersIds.Length; i++)
        {
            if (playersIds[i] == playerId)
                return i;
        }
        return -1;
    }

    public int[] GetPlayersIndex()
    {
        return playersIds;
    }

    public void PlayerAnim(int playerId, int idx)
    {
        if (!IsValidAnimIndex(idx))
            return;

        int animCode = animCodeList[idx];
        TriggerAnimation(playerId, animCode);
    }

    private bool IsValidAnimIndex(int idx)
    {
        return idx >= 0 && idx < animCodeList.Length;
    }

    public void SetPartyStatus(int playerId, int hp, int mp)
    {
        int playerIdx = GetPlayerIndexById(playerId);
        if (playerIdx == -1) return;

        float playerCurHp = hp;
        float playerCurMp = mp;

        teamInformation[playerIdx].SetCurHp(playerCurHp);
        teamInformation[playerIdx].SetCurMp(playerCurMp);

        if (playerId == GameManager.Instance.PlayerId)
        {
            MyInformation.SetCurHp(playerCurHp);
            MyInformation.SetCurMp(playerCurMp);
        }
    }

    public void BossMaterialChange(int randomElement)
    {
        int elementIndex = randomElement - Constants.PlayerCodeFactor;
        BossScript bossScript = monsterSpawnPos[0].GetChild(0).GetComponent<BossScript>();
        if (bossScript != null)
            bossScript.SetMaterial(elementIndex);
    }

    public void BossBarrierEnable()
    {
        UIMonsterInformation monsterInfo = monsterSpawnPos[0].GetComponentInChildren<UIMonsterInformation>();
        if (monsterInfo != null)
            monsterInfo.EnableBarrierImage();
    }

    public void BossBarrierBreak(int count)
    {
        UIMonsterInformation monsterInfo = monsterSpawnPos[0].GetComponentInChildren<UIMonsterInformation>();
        if (monsterInfo != null)
            monsterInfo.BreakBarrierImage(count);
    }
}
