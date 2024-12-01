using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEffectManager : MonoBehaviour
{
    private static BossEffectManager _instance = null;
    public static BossEffectManager Instance => _instance;

    [SerializeField] private GameObject[] effects;
    [SerializeField] private Transform myPlayerPos;

    //TODO: 몬스터가 때리는 유저에 맞게 유저가 맞을 수 있도록 변경하기
    [HideInInspector] private Transform[] playerPos;

    private int singleSkillIndex = 21;

    private int bufSkillIndex = 27;

    void Awake()
    {
        _instance = this;
    }

    public void SetEffectToPlayer(int code)
    {
        SetEffect(myPlayerPos, code);
    }

    public void SetEffectToMonster(int[] monsterIdx, int code)
    {
        var monster = BattleManager.Instance.GetMonster(monsterIdx);
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
}
