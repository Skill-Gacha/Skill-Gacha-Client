using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class VillageHead : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image fKey;

    private void OnTriggerStay(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (other.tag == "Player" && GameManager.Instance.UserName.Equals(player.GetNickname()))
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
