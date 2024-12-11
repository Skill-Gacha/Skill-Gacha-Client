using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvpEffectManager : MonoBehaviour
{
    private static PvpEffectManager _instance = null;

    // 싱글톤으로 나 자신 참조 중
    public static PvpEffectManager Instance => _instance;

    // 스킬을 사용하면 나타나는 효과 목록
    [SerializeField] private GameObject[] effects;

    // 나와 상대 중 나의 위치
    [SerializeField] private Transform playerTrans;

    // 나와 상대 중 상대의 위치
    [SerializeField] private Transform oppoentTrans;

    // 전체 스킬 개수 현재 총 27개
    int fullSkillIdx = 27;

    void Awake()
    {
        _instance = this;
    }


    public void SetEffectToPlayer(int code, bool isMyPlayer)
    {
        // code가 3028이하일 경우 상대방 공격 및 디버프 코드이고, 초과일 경우 본인 버프
        Transform trans = isMyPlayer ?  oppoentTrans : playerTrans;
        SetEffect(trans, code);
    }

    void SetEffect(Transform tr, int code)
    {
        // 뒤의 ?? 두개는 앞의 값이 null일 경우 기본적으로 이 값으로 처리하겠다는 의미 입니다.
        int calcId = code - Constants.EffectCodeFactor;
        // 효과 코드와 효과 코드 개수를 나타내는 상수와 빼
        // 배열의 효과 index를 가져온다.
        if(calcId < 0 || calcId >= effects.Length)
            return;

        //효과가 발생할 위치 설정
        Vector3 pos = new Vector3(tr.position.x, effects[calcId].transform.position.y, tr.position.z);
        effects[calcId].transform.position = pos;

        //효과 출력
        effects[calcId].gameObject.SetActive(false);
        //위의 코드 존재 이유는 여러 번 동작 되는 효과가 있어서
        //껏다 키는 것으로 추정 됩니다.
        effects[calcId].gameObject.SetActive(true);
    }
}
