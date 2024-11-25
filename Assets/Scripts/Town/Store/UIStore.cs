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

    [SerializeField] private TMP_Text txtPurchaseStatus;

    [SerializeField] private TMP_Text[] txtReserveAndLimit;
    [SerializeField] private TMP_Text[] txtPrice;

    [SerializeField] private TMP_Text txtGold;
    [SerializeField] private TMP_Text txtStone;
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
        txtPurchaseStatus.gameObject.SetActive(false);
        txtGold.text =  string.Format("{0:n0}", openStore.Gold.ToString());
        txtStone.text = string.Format("{0:n0}", openStore.Stone.ToString());
        var products = openStore.ProductList.ToArray();
        for(int i = 0; i < txtReserveAndLimit.Length; i++)
        {
            txtReserveAndLimit[i].text = products[i].Reserve + " / 3";
            txtPrice[i].text = products[i].Price.ToString();
        }
    }

    public void BuyItem(S_BuyItemResponse buyItem)
    {
        txtPurchaseStatus.gameObject.SetActive(true);
        txtPurchaseStatus.text = buyItem.Success ? "구매 성공!!" : "구매 실패...";
        txtPurchaseStatus.color = buyItem.Success ? new Color32(255,255,255,255) : new Color32(255,0,0,255);
        if(!buyItem.Success) return;

        int itemIdx = buyItem.ItemId - Constants.ItemCodeFactor - 1;
        txtGold.text =  string.Format("{0:n0}", buyItem.ChangeGold.ToString());
        txtReserveAndLimit[itemIdx].text = buyItem.Reserve + " / 3";
    }

    private void BuyProduct(int idx)
    {
        C_BuyItemRequest buyItem = new C_BuyItemRequest { ItemId = idx + Constants.ItemCodeFactor };
        GameManager.Network.Send(buyItem);
    }
}
