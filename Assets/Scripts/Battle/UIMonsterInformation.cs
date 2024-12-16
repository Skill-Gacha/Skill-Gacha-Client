using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterInformation : MonoBehaviour
{
    private Transform camTr;

    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtHp;
    [SerializeField] private Image imgHpFill;
    [SerializeField] private GameObject[] barrier;
    [SerializeField] private GameObject[] barrierObject;
    private float fullHP;
    private float curHP;

    public float fillWidth = 180;
    public float fillHeight = 30;

    private void Start()
    {
        camTr = Camera.main.transform;
    }

    public void SetName(string nickname)
    {
        txtName.text = nickname;
    }

    public void SetFullHp(float hp, bool recover = true)
    {
        fullHP = hp;
        txtHp.text = hp.ToString("0");

        if (recover)
            SetCurHp(hp);
    }

    public void SetCurHp(float hp)
    {
        curHP = Mathf.Min(hp, fullHP);
        txtHp.text = hp.ToString("0");
        float per = curHP/fullHP;
        imgHpFill.rectTransform.sizeDelta = new Vector2(fillWidth * per, fillHeight);
    }

    public void EnableBarrierImage()
    {
        for(int i = 0; i < barrier.Count();i++)
        {
            barrier[i].SetActive(true);
            barrierObject[i].SetActive(true);
        }
    }

    public void BreakBarrierImage(int index)
    {
        barrier[index].SetActive(false);
        barrierObject[index].SetActive(false);
    }
}
