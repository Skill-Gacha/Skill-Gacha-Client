using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UIBattlePopup : MonoBehaviour
{
    [SerializeField] private Button[] btns;

    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i + 1;
            btns[i].onClick.AddListener(() =>
            {
                EnterDungeon(idx);
            });
        }
    }

    private void EnterDungeon(int idx)
    {
        C_EnterDungeon enterPacket = new C_EnterDungeon { DungeonCode = idx };
        GameManager.Network.Send(enterPacket);
    }
}
