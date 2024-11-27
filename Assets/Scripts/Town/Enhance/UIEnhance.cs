using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIEnhance : MonoBehaviour
{
    [SerializeField] private Button[] btns;

    [SerializeField] private TMP_Text[] txtSkills;

    [SerializeField] private TMP_Text txtNotice;

    [SerializeField] private TMP_Text txtEnhance;

    [SerializeField] private Button btnEnhance;

    [SerializeField] private TMP_Text txtCostGold;
    [SerializeField] private TMP_Text txtCostStone;

    [SerializeField] private TMP_Text txtMyGold;
    [SerializeField] private TMP_Text txtMyStone;

    private string[] rankNames = { "[노말]", "[레어]", "[에픽]", "[유니크]", "[레전더리]" };
    private string[] costGoldList = { "1,000", "3,000", "5,000", "10,000" };
    private string[] costStoneList = { "5", "20", "30", "50" };
    private List<int> skillCodeList = new List<int>();
    private int ChoosenSkill = -1;

    private bool alreadyHaveSkill = false;

    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i + 1;
            btns[i].onClick.AddListener(() =>
            {
                SkillChoice(idx);
            });
        }
    }

    public void ShowEnhanceUi(S_EnhanceUiResponse openEnhance)
    {
        skillCodeList.Clear();
        // 강화 비용 초기화
        txtCostGold.text = "0";
        txtCostStone.text = "0";
        // 강화 안내 문구
        txtNotice.text = "강화할 스킬을 선택해 주세요.";
        txtNotice.color = new Color32(255, 255, 255, 255);
        // 보유 재화
        txtMyGold.text = openEnhance.Gold.ToString("N0");
        txtMyStone.text = openEnhance.Stone.ToString("N0");
        //보유 스킬
        var skillCodes = openEnhance.SkillCode.ToArray();
        for (int i = 0; i < skillCodes.Length; i++)
        {
            int skillCode = skillCodes[i];
            skillCodeList.Add(skillCode);
            var skillData = SkillDataManager.GetSkillById(skillCode);
            string skillName = skillData.skillName;
            int skillRank = skillData.rank;
            txtSkills[i].text = skillName + "\n" + rankNames[skillRank - 100];
        }
        //for (int i = 0; i < skillCodeList.Count; i++) Debug.Log(skillCodeList[i]);
    }

    private void SkillChoice(int idx)
    {
        ChoosenSkill = skillCodeList[idx - 1];
        // 상위 스킬을 보유하고 있는지 확인
        int nextSkill = ChoosenSkill + 5;
        if (skillCodeList.Contains(nextSkill))
        {
            alreadyHaveSkill = true;
        }
        else
        {
            alreadyHaveSkill = false;
        }

        int targetSkillRank = SkillDataManager.GetSkillById(ChoosenSkill).rank;
        // 강화 가능 여부 및 비용 표시
        if (targetSkillRank < 104 && alreadyHaveSkill == false)
        {
            txtEnhance.text = "강화가 가능한 스킬입니다.";
            txtEnhance.color = new Color32(255, 255, 255, 255);
            txtCostGold.text = costGoldList[targetSkillRank - 100];
            txtCostStone.text = costStoneList[targetSkillRank - 100];
            
        }
        else if (targetSkillRank == 104)
        {
            txtEnhance.text = "강화가 불가능한 스킬입니다.";
            txtEnhance.color = new Color32(255, 122, 119, 255);
            txtCostGold.text = "-";
            txtCostStone.text = "-";
        }
        else if(alreadyHaveSkill == true)
        {
            txtEnhance.text = "이미 상위 스킬을 보유 중입니다.";
            txtEnhance.color = new Color32(255, 122, 119, 255);
            txtCostGold.text = "-";
            txtCostStone.text = "-";
        }
    }

    public void WantEnhance()
    {
        if (alreadyHaveSkill) return;
        C_EnhanceRequest targetSkill = new C_EnhanceRequest { SkillCode = ChoosenSkill };
        GameManager.Network.Send(targetSkill);
    }

    public void EnhanceSuccess(S_EnhanceResponse enhanceSuccess)
    {
        // 강화 성공 여부 출력
        txtEnhance.gameObject.SetActive(true);
        txtEnhance.text = enhanceSuccess.Success ? "<강화 성공>" : "<강화 실패>";
        txtEnhance.color = enhanceSuccess.Success ? new Color32(162, 234, 255, 255) : new Color32(255, 122, 119, 255);
        if (!enhanceSuccess.Success) return;
    }
}
