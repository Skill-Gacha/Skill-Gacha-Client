using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RankData : MonoBehaviour
{
 [SerializeField]
    private TextMeshProUGUI textRank;
    [SerializeField]
    private TextMeshProUGUI textNickName;
    [SerializeField]
    private TextMeshProUGUI textScore;

    private int rank;
    private string nickname;
    private int score;

    public int Rank
    {
        set
        {
            if(value <= Constants.MaxRankList)
            {
                rank = value;
                textRank.text = rank.ToString();
            }
            else
            {
                textRank.text = "순위에 없음";
            }
        }
    }

    public string Nickname
    {
        set
        {
            nickname = value;
            textNickName.text = nickname;
        }
        get => nickname;
    }

    public int Score
    {
        set
        {
            score = value;
            textScore.text = score.ToString();
        }
        get => score;
    }
}
