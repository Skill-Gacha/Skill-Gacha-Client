using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Material[] limbosMaterials;
    [SerializeField]
    private Material[] spineMaterials;
    [SerializeField]
    private GameObject dragon;

    public void SetMaterial(int elementIndex)
    {
        Renderer renderer = dragon.GetComponent<Renderer>();
        
        // 현재 오브젝트에 있는 모든 Material을 가져옵니다.
        Material[] materials = renderer.materials;

        // 배열이 두 개 이상의 Material을 포함하는지 확인합니다.
        if (materials.Length >= 2)
        {
            materials[0] = limbosMaterials[elementIndex]; // 첫 번째 Material
            materials[1] = spineMaterials[elementIndex]; // 두 번째 Material
        }
        else
        {
            Debug.LogWarning("dragon 오브젝트에 Material 슬롯이 2개 이상 필요합니다.");
        }

        // 업데이트된 Material 배열을 다시 설정합니다.
        renderer.materials = materials;
    }
}