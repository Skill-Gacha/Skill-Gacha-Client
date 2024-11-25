using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStore : MonoBehaviour
{
    [SerializeField] private Button[] btns;
    [SerializeField] private TMP_Text[] txtReserveAndLimit;
    [SerializeField] private TMP_Text[] txtPrice;
    private void Start()
    {
        for (int i = 0; i < btns.Length; i++)
        {
            int idx = i + 1;
            btns[i].onClick.AddListener(() =>
            {
                BuyProduct(idx);
            });
        }
    }

    public void ShowStoreUi(S_OpenStoreResponse openStore)
    {
        int gold = openStore.Gold;
        int stone = openStore.Stone;
        var products = openStore.ProductList.ToArray();
        for(int i = 0; i < txtReserveAndLimit.Length; i++)
        {
            txtReserveAndLimit[i].text = products[i].Reserve + " / 3";
            txtPrice[i].text = products[i].Price.ToString();
        }
    }

    private void BuyProduct(int idx)
    {
        C_BuyItemRequest buyItem = new C_BuyItemRequest { ItemId = idx + 4000 };
        GameManager.Network.Send(buyItem);
    }
}
