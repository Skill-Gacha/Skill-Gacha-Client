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

    [SerializeField] private TMP_Text txtEnhance;

    [SerializeField] private Button btnEnhance;

    [SerializeField] private TMP_Text txtCostGold;
    [SerializeField] private TMP_Text txtCostStone;

    [SerializeField] private TMP_Text txtMyGold;
    [SerializeField] private TMP_Text txtMyStone;

    private string[] rankNames = { "[노말]", "[레어]", "[에픽]", "[유니크]", "[레전더리]" };
    private List<int> skillCodeList = new List<int>();

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
        Debug.Log(openEnhance);
        // 강화 비용 초기화
        txtCostGold.text = "0";
        txtCostStone.text = "0";
        // 강화 안내 문구
        txtEnhance.text = "강화할 스킬을 선택해 주세요.";
        txtEnhance.color = new Color32(255, 255, 255, 255);
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
    }

    private void SkillChoice(int idx)
    {
        int targetSkill = skillCodeList[idx - 1];
        int targetSkillRank = SkillDataManager.GetSkillById(targetSkill).rank;
        // 강화 비용 표시
        if (targetSkillRank == 100)
        {
            txtEnhance.text = "강화가 가능한 스킬입니다.";
            txtCostGold.text = "1,000";
            txtCostStone.text = "5";
        }
        if (targetSkillRank == 101)
        {
            txtEnhance.text = "강화가 가능한 스킬입니다.";
            txtCostGold.text = "3,000";
            txtCostStone.text = "20";
        }
        if (targetSkillRank == 102)
        {
            txtEnhance.text = "강화가 가능한 스킬입니다.";
            txtCostGold.text = "5,000";
            txtCostStone.text = "30";
        }
        if (targetSkillRank == 103)
        {
            txtEnhance.text = "강화가 가능한 스킬입니다.";
            txtCostGold.text = "10,000";
            txtCostStone.text = "50";
        }
        if (targetSkillRank == 104)
        {
            txtEnhance.text = "강화가 불가능한 스킬입니다.";
            txtCostGold.text = "-";
            txtCostStone.text = "-";
        }
        // 스킬을 선택해 둔 상태에서만 강화 버튼 클릭 이벤트 감지
        btnEnhance.onClick.AddListener(() =>
        {
            EnhanceSkillRequest(targetSkill);
        });
    }
    private void EnhanceSkillRequest(int targetSkillCode)
    {
        // 서버에 스킬 강화 요청
        C_EnhanceRequest targetSkill = new C_EnhanceRequest { SkillCode = targetSkillCode };
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
