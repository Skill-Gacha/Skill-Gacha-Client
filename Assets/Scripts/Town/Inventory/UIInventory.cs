using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    //[SerializeField] private Button[] btns;
    [SerializeField] private TMP_Text[] txtReserveAndLimit;
    [SerializeField] private TMP_Text txtGold;
    [SerializeField] private TMP_Text txtStone;

    public void ShowInventoryUi(S_InventoryViewResponse openInventory)
    {
        txtGold.text = openInventory.Gold.ToString("N0");
        txtStone.text = openInventory.Stone.ToString("N0");

        var products = openInventory.ProductList.ToArray();
        for (int i = 0; i < products.Length; i++)
        {
            int slashIndex = txtReserveAndLimit[i].text.IndexOf('/');
            txtReserveAndLimit[i].text = products[i].Reserve + " " + txtReserveAndLimit[i].text.Substring(slashIndex).Trim();
        }
    }
}
