using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Google.Protobuf.Protocol;
using System.Linq;

public class UIRank : MonoBehaviour
{
    [SerializeField]
    private Transform context;

    [SerializeField]
    private GameObject rankTempObj;

    [SerializeField]
    private TMP_Text txtMyRank;

    [SerializeField]
    private TMP_Text txtMyNickname;

    [SerializeField]
    private TMP_Text txtMyScore;

    public void ViewRankUi(S_ViewRankPoint viewRank)
    {
        // 기존 목록 지우기
        foreach(Transform child in context)
        {
            Destroy(child.gameObject);
        }

        Rank[] other = viewRank.OtherRanks.ToArray();

        // 새 목록 추가하기
        for(int i = 0; i < other.Length; i++)
        {
            Debug.Log(other[i].PlayerName+", "+other[i].PlayerRank+", "+other[i].PlayerName);
            RankData rankData = Instantiate(rankTempObj).GetComponent<RankData>();
            rankData.Score = other[i].PlayerScore;
            rankData.Rank = other[i].PlayerRank;
            rankData.Nickname = other[i].PlayerName;
            rankData.gameObject.transform.SetParent(context,false);
        }

        // 내 랭킹, 점수 넣기
        Rank myRanking = viewRank.MyRank;
        txtMyRank.text = myRanking.PlayerRank.ToString("N0");
        txtMyNickname.text = myRanking.PlayerName;
        txtMyScore.text = myRanking.PlayerScore.ToString("N0");
    }
}
