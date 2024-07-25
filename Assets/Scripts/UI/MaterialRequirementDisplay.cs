using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialRequirementDisplay : MonoBehaviour
{
    public GameObject materialIconPrefab; // 재료 아이콘을 표시할 프리팹
    public List<GameObject> slots; // 미리 생성된 슬롯 리스트

    public void DisplayMaterialRequirements(ItemKind itemInfo)
    {
        // 모든 슬롯을 비활성화
        foreach (var slot in slots)
        {
            foreach (Transform child in slot.transform)
            {
                Destroy(child.gameObject);
            }
            slot.SetActive(false);
        }

        // 아이템의 다음 레벨을 위한 재료 요구사항 가져오기
        List<MaterialRequirement> materialRequirements = null;

        if (itemInfo is WeaponItem weapon)
        {
            materialRequirements = weapon.GetMaterialRequirementsForLevel(weapon.kaiLevel + 1);
        }
        else if (itemInfo is ArmorItem armor)
        {
            materialRequirements = armor.GetMaterialRequirementsForLevel(armor.kaiLevel + 1);
        }

        if (materialRequirements == null) return;

        // 필요한 각 재료에 대해 아이콘 생성
        for (int i = 0; i < materialRequirements.Count && i < slots.Count; i++)
        {
            var material = materialRequirements[i];
            var slot = slots[i];

            // 슬롯을 활성화
            slot.SetActive(true);

            // 프리팹 인스턴스화
            GameObject materialIconInstance = Instantiate(materialIconPrefab, slot.transform);

            // 재료 정보를 표시하는 로직 (예: 아이콘 이미지, 이름, 수량 등)
            Image iconImage = materialIconInstance.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = material.requiredItem.itemIcon; // 아이콘 이미지 설정
            }

            TextMeshProUGUI quantityText = materialIconInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (quantityText != null)
            {
                quantityText.text = material.quantity.ToString(); // 수량 설정
            }
        }
    }
}