using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private static BattleManager _instance = null;
    public static BattleManager Instance => _instance;
    
    [SerializeField] private UIScreen uiScreen;
    [SerializeField] private UIBattleLog uiBattleLog;
    [SerializeField] private UIPlayerInformation uiPlayerInformation;

    public UIScreen UiScreen => uiScreen;
    public UIBattleLog UiBattleLog => uiBattleLog;
    public UIPlayerInformation UiPlayerInformation => uiPlayerInformation;
    
    [SerializeField] private Maps map;
    
    [SerializeField] private Transform[] players;
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

        Set(GameManager.Instance.Pkt);
        GameManager.Instance.Pkt = null;
    }

    public void Set(S_EnterDungeon pkt)
    {
        if (pkt.DungeonInfo != null)
            SetDungeon(pkt.DungeonInfo);

        if (pkt.Player != null)
        {
            uiPlayerInformation.Set(pkt.Player);
            SetPlayer(pkt.Player.PlayerClass);
        }

        if (pkt.ScreenText != null)
            uiScreen.Set(pkt.ScreenText);

        if (pkt.BattleLog != null)
            uiBattleLog.Set(pkt.BattleLog);
    }

    private void SetPlayer(int classCode)
    {
        int idx = classCode - Constants.PlayerCodeFactor;
        for (int i = 0; i < players.Length; i++)
        {
            bool select = i == idx;
            players[i].gameObject.SetActive(select);
            
            if (select)
                playerAnimator = players[i].GetComponent<Animator>();
        }
    }

    public void SetDungeon(DungeonInfo dungeonInfo)
    {
        SetMap(dungeonInfo.DungeonCode);
        SetMonster(dungeonInfo.Monsters);
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

    public void PlayerHit()
    {
        TriggerAnim(Constants.PlayerBattleHit);
    }

    public void PlayerAnim(int idx)
    {
        if(idx < 0 || idx >= animCodeList.Length)
            return;
        
        var animCode = animCodeList[idx];
        TriggerAnim(animCode);
    }

    void TriggerAnim(int code)
    {
        playerAnimator.transform.localEulerAngles = Vector3.zero;
        playerAnimator.transform.localPosition = Vector3.zero;
        playerAnimator.applyRootMotion = code == Constants.PlayerBattleDie;
        playerAnimator.SetTrigger(code);
    }

    public void SetMap(int id)
    {
        map.SetMap(id);
    }
}
