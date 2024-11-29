using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class RaidManager : MonoBehaviour
{
    private static RaidManager _instance = null;
    public static RaidManager Instance => _instance;
    
    [SerializeField] private RaidUIScreen uiScreen;
    [SerializeField] private RaidBattleLog uiBattleLog;
    [SerializeField] private RaidUIPlayerInformation myInformation;

    public RaidUIScreen UiScreen => uiScreen;
    public RaidBattleLog UiBattleLog => uiBattleLog;
    public RaidUIPlayerInformation MyInformation => myInformation;

    [SerializeField] private Maps map;

    [SerializeField] private Transform[] myPlayerTrans;

    [SerializeField] private Transform[] myTeamLeft;

    [SerializeField] private Transform[] myTeamRight;

    private Animator playerAnimator;

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

        Set(GameManager.Instance.Raid);
        GameManager.Instance.Raid = null;
    }

    public void Set(S_BossMatchNotification Raid)
    {
        SetMap(Raid.DungeonCode);

        if (Raid.Player != null)
        {
            MyInformation.Set(Raid.Player);
            SetPlayer(Raid.Player.PlayerClass);
        }

        if(Raid.Member != null)
        {
            //
        }

        // if()
        // if (Raid.BattleLog != null)
        //     uiBattleLog.Set(Raid.BattleLog);
    }

    private void SetPlayer(int classCode)
    {
        int idx = classCode - Constants.PlayerCodeFactor;
        for (int i = 0; i < myPlayerTrans.Length; i++)
        {
            bool select = i == idx;
            myPlayerTrans[i].gameObject.SetActive(select);
            if (select)
                playerAnimator = myPlayerTrans[i].GetComponent<Animator>();
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

    // public List<Monster> GetMonster(int[] monsterIndex)
    // {
    //     return monsterIndex.Where(index => index >= 0 && index < monsterObjs.Count).Select(index => monsterObjs[index]).ToList();
    // }

    public void PlayerHit()
    {
        TriggerAnim(Constants.PlayerBattleHit);
    }

    void TriggerAnim(int code)
    {
        playerAnimator.transform.localEulerAngles = Vector3.zero;
        playerAnimator.transform.localPosition = Vector3.zero;
        playerAnimator.applyRootMotion = code == Constants.PlayerBattleDie;
        playerAnimator.SetTrigger(code);
    }

    public void PlayerAnim(int idx)
    {
        if(idx < 0 || idx >= animCodeList.Length)
            return;

        var animCode = animCodeList[idx];
        TriggerAnim(animCode);
    }

    public void SetMap(int id)
    {
        map.SetMap(id);
    }
}
