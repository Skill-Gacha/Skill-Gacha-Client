using System;
using System.Collections.Generic;
using UnityEngine;

public static class SkillDataManager
{
    public static SkillClass SkillData { get; private set; }

    public static void LoadSkillData()
    {
        // JSON ���� �ε�
        TextAsset textAsset = Resources.Load<TextAsset>("Data/skillData");
        if (textAsset == null)
        {
            Debug.LogError("skillData.json ������ ã�� �� �����ϴ�!");
            return;
        }

        // JSON �Ľ�
        SkillData = JsonUtility.FromJson<SkillClass>(textAsset.text);
        Debug.Log("Skill data �ε� �Ϸ�!");
    }

    // Ư�� ��ų ID�� ��ų �����͸� ã�� �޼���
    public static SkillData GetSkillById(int skillId)
    {
        if (SkillData == null)
        {
            Debug.LogError("SkillData�� �ε���� �ʾҽ��ϴ�. LoadSkillData()�� ȣ���ϼ���.");
            return null;
        }

        return SkillData.data.Find(s => s.id == skillId);
    }
}