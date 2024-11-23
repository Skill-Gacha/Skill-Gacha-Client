using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StoreKeeper : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image fKey;

    private void OnTriggerStay(Collider other)
    {
        // TODO:한 명의 유저만 감지될 수 있도록 변경하기
        if (other.tag == "Player")
        {
            fKey.gameObject.SetActive(true);
            fKey.transform.LookAt(Camera.main.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            fKey.gameObject.SetActive(false);
        }
    }
}
