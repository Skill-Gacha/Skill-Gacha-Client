using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGold : MonoBehaviour
{
    [SerializeField] private TMP_Text userGold;

    public void setGold(int gold)
    {
        userGold.text = gold.ToString();

    }
}
