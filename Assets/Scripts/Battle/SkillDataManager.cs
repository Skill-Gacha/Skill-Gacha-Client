using System;
using System.Collections.Generic;
using UnityEngine;

public static class SkillDataManager
{
    public static SkillClass SkillData { get; private set; }

    public static void LoadSkillData()
    {
        // JSON 파일 로드
        TextAsset textAsset = Resources.Load<TextAsset>("Data/skillData");
        if (textAsset == null)
        {
            Debug.LogError("skillData.json 파일을 찾을 수 없습니다!");
            return;
        }

        // JSON 파싱
        SkillData = JsonUtility.FromJson<SkillClass>(textAsset.text);
        Debug.Log("Skill data 로드 완료!");
    }

    // 특정 스킬 ID로 스킬 데이터를 찾는 메서드
    public static SkillData GetSkillById(int skillId)
    {
        if (SkillData == null)
        {
            Debug.LogError("SkillData가 로드되지 않았습니다. LoadSkillData()를 호출하세요.");
            return null;
        }

        return SkillData.data.Find(s => s.id == skillId);
    }
}