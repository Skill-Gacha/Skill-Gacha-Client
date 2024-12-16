// ----- G:\Camp\MainCamp\Final\Skill-Gacha-Client\Assets\Scripts\Boss\BossScript.cs 리팩터링 -----

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    [SerializeField] private Material[] limbosMaterials;
    [SerializeField] private Material[] spineMaterials;
    [SerializeField] private GameObject dragon;

    [SerializeField] private Image imgElement;
    [SerializeField] private Sprite[] elements;

    public void SetMaterial(int elementIndex)
    {
        Renderer renderer = dragon.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Renderer component not found on dragon.");
            return;
        }

        if (!imgElement.gameObject.activeSelf)
            imgElement.gameObject.SetActive(true);

        if (IsValidElementIndex(elementIndex))
        {
            imgElement.sprite = elements[elementIndex];
        }
        else
        {
            Debug.LogWarning($"Invalid elementIndex: {elementIndex}");
            return;
        }

        UpdateMaterials(renderer, elementIndex);
    }

    private bool IsValidElementIndex(int index)
    {
        return index >= 0 && index < limbosMaterials.Length && index < spineMaterials.Length && index < elements.Length;
    }

    private void UpdateMaterials(Renderer renderer, int elementIndex)
    {
        Material[] materials = renderer.materials;

        if (materials.Length >= 2)
        {
            materials[0] = limbosMaterials[elementIndex]; // 첫 번째 Material
            materials[1] = spineMaterials[elementIndex];   // 두 번째 Material
            renderer.materials = materials;
        }
        else
        {
            Debug.LogWarning("dragon 오브젝트에 Material 슬롯이 2개 이상 필요합니다.");
        }
    }
}