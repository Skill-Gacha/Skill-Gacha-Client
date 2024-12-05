using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIBossMatching : MonoBehaviour
{
    public void ShowBossMatchingUi()
    {
        this.gameObject.SetActive(true);
    }

    public void BossPartyResponse(bool success)
    {
        C_AcceptResponse acceptResponse = new C_AcceptResponse { Accept = success };
        GameManager.Network.Send(acceptResponse);
    }

    public void StopMatch()
    {
        this.gameObject.SetActive(false);
    }
}