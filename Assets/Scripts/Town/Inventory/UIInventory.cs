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
    //[SerializeField] private TMP_Text[] txtPrice;
    private void Start()
    {
        //for (int i = 0; i < btns.Length; i++)
        //{
        //    int idx = i + 1;
        //    btns[i].onClick.AddListener(() =>
        //    {
        //        BuyProduct(idx);
        //    });
        //}
    }

    public void ShowInventoryUi(S_InventoryViewResponse openInventory)
    {
        int gold = openInventory.Gold;
        int stone = openInventory.Stone;
        var products = openInventory.ProductList.ToArray();
        for (int i = 0; i < txtReserveAndLimit.Length; i++)
        {
            if(i<3)
            {
                txtReserveAndLimit[i].text = products[i].Reserve + " / 3";
            }
            else
            {
                txtReserveAndLimit[i].text = products[i].Reserve + " / 1";
            }
        }
    }
}
