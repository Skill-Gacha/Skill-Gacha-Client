// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossEffectManager.cs 리팩터링 -----

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEffectManager : MonoBehaviour
{
    private static BossEffectManager _instance = null;
    public static BossEffectManager Instance => _instance;

    [SerializeField] private GameObject[] effects;
    [SerializeField] private Transform[] playerPos;

    private const int SingleSkillIndex = 21;
    private const int BufSkillIndex = 27;

    private void Awake()
    {
        _instance = this;
    }

    // 플레이어들에게 단일 또는 버프 효과를 적용
    public void SetEffectToPlayer(int code, int playerId = -1, bool isSingleSkill = true)
    {
        if (isSingleSkill)
        {
            SetSingleEffect(code);
        }
        else
        {
            SetBuffEffect(code, playerId);
        }
    }

    // 단일 스킬 효과 적용
    private void SetSingleEffect(int code)
    {
        SetEffect(code);
    }

    // 버프 스킬 효과 적용
    private void SetBuffEffect(int code, int playerId)
    {
        int index = BossManager.Instance.GetPlayerIndexById(playerId);
        if (index == -1) return;

        SetEffect(playerPos[index], code);
    }

    // 몬스터에게 효과 적용
    public void SetEffectToMonster(int[] monsterIdx, int code)
    {
        var monsters = BossManager.Instance.GetMonster(monsterIdx);
        foreach (var monster in monsters)
        {
            if (monster != null)
                SetEffect(monster.transform, code);
        }
    }

    private void SetEffect(Transform tr, int code)
    {
        int calcId = code - Constants.EffectCodeFactor;
        if (!IsValidEffectId(calcId))
            return;

        Vector3 pos = new Vector3(tr.position.x, effects[calcId].transform.position.y, tr.position.z);
        PlayEffect(effects[calcId], pos);
    }

    private void SetEffect(int code)
    {
        int calcId = code - Constants.EffectCodeFactor;
        if (!IsValidEffectId(calcId))
            return;

        PlayEffect(effects[calcId], effects[calcId].transform.position);
    }

    private bool IsValidEffectId(int calcId)
    {
        return calcId >= 0 && calcId < effects.Length;
    }

    private void PlayEffect(GameObject effect, Vector3 position)
    {
        effect.transform.position = position;
        effect.SetActive(false);
        effect.SetActive(true);
    }
}
