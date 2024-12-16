// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossUIMatching.cs �����͸� -----

using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossMatching : MonoBehaviour
{
    bool check = false;
    public void ShowBossMatchingUi()
    {
        gameObject.SetActive(true);
        check = false;
        Debug.Log("check : "+check);
    }

    public void BossPartyResponse(bool success)
    {
        Debug.Log("check : "+check);
        if(!check)
        {
            C_AcceptResponse acceptResponse = new C_AcceptResponse { Accept = success };
            GameManager.Network.Send(acceptResponse);
            check = true;
        }
    }

    public void StopMatch()
    {
        gameObject.SetActive(false);
        check = false;
        Debug.Log("check : "+check);
    }
}