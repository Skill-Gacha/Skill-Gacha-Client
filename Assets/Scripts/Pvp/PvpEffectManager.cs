using UnityEngine;

public class PvpEffectManager : MonoBehaviour
{
    private static PvpEffectManager _instance = null;
    public static PvpEffectManager Instance => _instance;

    [SerializeField] private GameObject[] effects;
    [SerializeField] private Transform playerTrans;
    [SerializeField] private Transform oppoentTrans;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
    }

    public void SetEffectToPlayer(int code, bool isMyPlayer)
    {
        Transform targetTrans = isMyPlayer ? oppoentTrans : playerTrans;
        ApplyEffect(targetTrans, code);
    }

    private void ApplyEffect(Transform target, int code)
    {
        int calcId = code - Constants.EffectCodeFactor;
        if (IsValidEffect(calcId))
        {
            ActivateEffect(target, calcId);
        }
    }

    private bool IsValidEffect(int calcId)
    {
        return calcId >= 0 && calcId < effects.Length;
    }

    private void ActivateEffect(Transform target, int effectId)
    {
        Vector3 pos = new Vector3(target.position.x, effects[effectId].transform.position.y, target.position.z);
        GameObject effect = effects[effectId];
        effect.transform.position = pos;

        // Recycle the effect to allow multiple activations
        effect.SetActive(false);
        effect.SetActive(true);
    }
}