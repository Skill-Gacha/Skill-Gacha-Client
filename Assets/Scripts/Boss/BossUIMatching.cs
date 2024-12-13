// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossUIMatching.cs ∏Æ∆—≈Õ∏µ -----

using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossMatching : MonoBehaviour
{
    public void ShowBossMatchingUi()
    {
        gameObject.SetActive(true);
    }

    public void BossPartyResponse(bool success)
    {
        C_AcceptResponse acceptResponse = new C_AcceptResponse { Accept = success };
        GameManager.Network.Send(acceptResponse);
    }

    public void StopMatch()
    {
        gameObject.SetActive(false);
    }
}