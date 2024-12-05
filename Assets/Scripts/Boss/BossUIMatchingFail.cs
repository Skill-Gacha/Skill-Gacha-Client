using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIBossMatchingFail : MonoBehaviour
{
    public void ShowBossMatchingFailUi()
    {
        this.gameObject.SetActive(true);
    }
}