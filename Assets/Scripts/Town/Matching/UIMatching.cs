using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMatching : MonoBehaviour
{
    [SerializeField] private Transform progress;

    public void StartMatch() {
        this.gameObject.SetActive(true);
        StartCoroutine(SpinProgress());
    }

    IEnumerator SpinProgress()
    {
        while(true)
        {
            progress.transform.localEulerAngles = new Vector3(0, 0, progress.transform.localEulerAngles.z - 1);
            yield return null;
        }

    }

    public void StopMatch()
    {
        StopCoroutine(SpinProgress());
        this.gameObject.SetActive(false);
    }
}
