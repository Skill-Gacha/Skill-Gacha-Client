using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMatching : MonoBehaviour
{
    [SerializeField] private Transform progress;
    private bool isMatching = false;
    public void ViewMatch() {
        StartCoroutine(SpinProgress());
    }

    IEnumerator SpinProgress()
    {
        while(true)
        {
            Debug.Log("확인");
            progress.transform.localEulerAngles = new Vector3(0, 0, progress.transform.localEulerAngles.z - 1);
            yield return new WaitUntil(()=> !isMatching);
            isMatching = !isMatching;
            this.gameObject.SetActive(isMatching);
        }
    }

    public void setMatching(bool isMatching)
    {
        this.isMatching = isMatching;
    }

    public bool getMatching()
    {
        return this.isMatching;
    }
}
