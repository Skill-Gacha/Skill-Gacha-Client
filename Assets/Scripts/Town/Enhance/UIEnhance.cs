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

    private struct SkillRank
    {
        public string RankName;
        public string CostGold;
        public string CostStone;

        public SkillRank(string rankName, string costGold, string costStone)
        {
            RankName = rankName;
            CostGold = costGold;
            CostStone = costStone;
        }
    }

    private SkillRank[] skillRanks =
    {
        new SkillRank("[노말]", "1,000","5"),
        new SkillRank("[레어]", "3,000","20"),
        new SkillRank("[에픽]", "5,000","30"),
        new SkillRank("[유니크]", "10,000","50"),
        new SkillRank("[레전더리]", "-","-"),
    };

    private List<int> skillCodeList = new List<int>();
    private int choosenSkill = -1;

    private bool alreadyHaveSkill = false;

    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i + 1;
            btns[i].onClick.AddListener(() => SkillChoice(idx));
        }
    }

    private void ResetEnhanceUI()
    {
        for(int i = 0; i < btns.Length; i++)
        {
            btns[i].gameObject.SetActive(false);
        }
        skillCodeList.Clear();
        txtCostGold.text = "-";
        txtCostStone.text = "-";
        txtNotice.text = "강화할 스킬을 선택해 주세요.";
        txtNotice.color = new Color32(255, 255, 255, 255);
        txtEnhance.gameObject.SetActive(false);
    }

    public void ShowEnhanceUi(S_EnhanceUiResponse openEnhance)
    {
        ResetEnhanceUI();
        txtMyGold.text = openEnhance.Gold.ToString("N0");
        txtMyStone.text = openEnhance.Stone.ToString("N0");

        var skillCodes = openEnhance.SkillCode.ToArray();

        for (int i = 0; i < skillCodes.Length; i++)
        {
            btns[i].gameObject.SetActive(true);
            int skillCode = skillCodes[i];
            skillCodeList.Add(skillCode);
            SetSkillInfo(i, skillCode);
        }
    }

    private void SetSkillInfo(int index, int skillCode)
    {
        var skillData = SkillDataManager.GetSkillById(skillCode);
        txtSkills[index].text = $"{skillData.skillName}\n{skillRanks[skillData.rank - 100].RankName}";
    }

    private void SkillChoice(int idx)
    {
        choosenSkill = skillCodeList[idx - 1];
        alreadyHaveSkill = skillCodeList.Contains(choosenSkill + 5);

        int targetSkillRank = SkillDataManager.GetSkillById(choosenSkill).rank;
        if(targetSkillRank < 104 && !alreadyHaveSkill)
        {
            SetEnhanceStatus("강화가 가능한 스킬입니다", new Color32(255, 255, 255, 255), skillRanks[targetSkillRank - 100]);
        }
        else if(targetSkillRank == 104)
        {
            SetEnhanceStatus("강화가 불가능한 스킬입니다.", new Color32(255, 122, 119, 255));
        }
        else if(alreadyHaveSkill)
        {
            SetEnhanceStatus("이미 상위 스킬을 보유 중입니다.", new Color32(255, 122, 119, 255));
        }
    }

    private void SetEnhanceStatus(string message, Color32 color, SkillRank? rank = null)
    {
        txtEnhance.gameObject.SetActive(true);
        txtEnhance.text = message;
        txtEnhance.color = color;
        txtCostGold.text = rank?.CostGold ?? "-";
        txtCostStone.text = rank?.CostStone ?? "-";
    }

    public void WantEnhance()
    {
        if (alreadyHaveSkill) return;
        else if(choosenSkill == -1)
        {
            txtEnhance.text = "강화할 대상을 선택해주세요";
            return;
        }
        C_EnhanceRequest targetSkill = new C_EnhanceRequest { SkillCode = choosenSkill };
        choosenSkill = -1;
        txtCostGold.text = "-";
        txtCostStone.text = "-";
        GameManager.Network.Send(targetSkill);
    }

    public void EnhanceSuccess(S_EnhanceResponse enhanceSuccess)
    {
        // 강화 성공 여부 출력
        txtEnhance.gameObject.SetActive(true);
        txtEnhance.text = enhanceSuccess.Success ? "<강화 성공>" : "<강화 실패>";
        txtEnhance.color = enhanceSuccess.Success ? new Color32(162, 234, 255, 255) : new Color32(255, 122, 119, 255);
    }
}
