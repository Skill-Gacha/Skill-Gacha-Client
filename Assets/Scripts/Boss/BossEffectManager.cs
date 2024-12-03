using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEffectManager : MonoBehaviour
{
    private static BossEffectManager _instance = null;
    public static BossEffectManager Instance => _instance;

    [SerializeField] private GameObject[] effects;

    [SerializeField] private Transform[] playerPos;

    private int singleSkillIndex = 21;

    private int bufSkillIndex = 27;

    void Awake()
    {
        _instance = this;
    }

    // 플레이어들이 단체 버프 쓸 경우 처리 함수
    public void SetEffectToPlayer(int code, bool isMonsterAttack)
    {
        for(int i = 0; i < playerPos.Count();i++)
        {
            SetEffect(playerPos[i], code);
        }
    }

    // 보스가 특정 대상에게 디버프 걸 경우
    public void SetEffectToPlayer(int playerId,int code)
    {
        int index = BossManager.Instance.GetPlayerIndexById(playerId);
        SetEffect(playerPos[index], code);
    }

    public void SetEffectToMonster(int[] monsterIdx, int code)
    {
        var monster = BossManager.Instance.GetMonster(monsterIdx);
        SetEffect(monster[0].transform, code);
    }

    void SetEffect(Transform tr, int code)
    {
        var calcId = code - Constants.EffectCodeFactor;
        Debug.Log("code : "+code);
        Debug.Log("calcId : "+calcId);
        if(calcId < 0 || calcId >= effects.Length)
            return;
        // 3022(index : 21) 아래 스킬들은 단일기, 3028(index : 27) 아래는 전체기 + 디버프
        if(calcId < singleSkillIndex || calcId > bufSkillIndex)
        {
            var pos = new Vector3(tr.position.x, effects[calcId].transform.position.y, tr.position.z);
            effects[calcId].transform.position = pos;
        }
        effects[calcId].gameObject.SetActive(false);
        effects[calcId].gameObject.SetActive(true);
    }

    void SetEffect(int code)
    {
        var calcId = code - Constants.EffectCodeFactor;
        Debug.Log("code : "+code);
        Debug.Log("calcId : "+calcId);
        if(calcId < 0 || calcId >= effects.Length)
            return;

        effects[calcId].gameObject.SetActive(false);
        effects[calcId].gameObject.SetActive(true);
    }
}
