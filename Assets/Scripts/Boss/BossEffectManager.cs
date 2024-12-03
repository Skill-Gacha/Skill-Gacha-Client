using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEffectManager : MonoBehaviour
{
    private static BossEffectManager _instance = null;
    public static BossEffectManager Instance => _instance;
    //private static Boss

    [SerializeField] private GameObject[] effects;

    [SerializeField] private Transform[] playerPos;

    private int singleSkillIndex = 21;

    private int bufSkillIndex = 27;

    void Awake()
    {
        _instance = this;
    }

    // 플레이어들이 단체 버프 쓸 경우 처리 함수

    public void SetEffectToPlayer(int code)
    {
        // 3페이지 전체 디버프인 경우
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

        if(calcId < 0 || calcId >= effects.Length)
            return;
        if(calcId >= 31)
        {
            BossEffect(calcId);
            return;
        }
        // 3022(index : 21) 아래 스킬들은 단일기, 3029(index : 28) ~ 3031(index : 30) 버프, 3032(index : 31)은 드래곤의 단일 Hp, Mp 역전 공격
        else if(calcId < singleSkillIndex || calcId > bufSkillIndex && calcId < 32)
        {
            var pos = new Vector3(tr.position.x, effects[calcId].transform.position.y, tr.position.z);
            effects[calcId].transform.position = pos;
        }

        // 3033(index : 32) ~ 3034(index : 33) 드래곤 광역기 공격 및 광역 디버프
        // 3023(index : 22) ~ 3028(index : 27) 유저들의 광역기
        effects[calcId].gameObject.SetActive(false);
        effects[calcId].gameObject.SetActive(true);
    }

    void BossEffect(int index)
    {
        effects[index].gameObject.SetActive(false);
        effects[index].gameObject.SetActive(true);
    }
}
