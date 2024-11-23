using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UIStore : MonoBehaviour
{
    [SerializeField] private Button[] btns;
    private Product[] products;
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
        Debug.Log("OpenStore : "+openStore);
        int gold = openStore.Gold;
        int stone = openStore.Stone;

        if(openStore.ProductList is { Count: > 0})
            products = openStore.ProductList?.ToArray();

        Debug.Log("gold : "+gold+", stone : "+stone);
        Debug.Log("product : "+products);
    }

    private void BuyProduct(int idx)
    {
        C_BuyItemRequest buyItem = new C_BuyItemRequest { ItemId = idx };
        GameManager.Network.Send(buyItem);
    }
}
