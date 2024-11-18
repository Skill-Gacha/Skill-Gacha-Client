using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    [SerializeField] private Button btnBattle;
    [SerializeField] private Button[] btnList;
    private MyPlayer mPlayer;
    
    void Start()
    {
        for (int i = 0; i < btnList.Length; i++)
        {
            int idx = i;
            btnList[i].onClick.AddListener(() =>
            {
                PlayAnimation(idx);
            });
        }
        
        mPlayer = TownManager.Instance.myPlayer.mPlayer;
    }
    
    private void PlayAnimation(int idx)
    {
        if (mPlayer == null)
            return;

        mPlayer.AnimationExecute(idx);
    }
}
