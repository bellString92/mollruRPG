using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;

public class MaterialRequirementDisplay : MonoBehaviour
{
    public GameObject materialIconPrefab; // 재료 아이콘을 표시할 프리팹
    public List<GameObject> slots; // 미리 생성된 슬롯 리스트

    public void DisplayMaterialRequirements(ItemKind itemInfo)
    {
        if (itemInfo != null)
        {  // 모든 슬롯을 초기화
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
                Image iconImage = materialIconInstance.GetComponentInChildren<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = material.requiredItem.itemIcon; // 아이콘 이미지 설정
                }

                //레어도에 따라 차이점
                Image rarityBG = materialIconInstance.AddComponent<Image>();
                switch (itemInfo.rarity)
                {
                    case Rarity.Common:
                        rarityBG.color = Color.clear;
                        break;
                    case Rarity.Uncommon:
                        rarityBG.color = new Color(0, 1, 0, 10f / 255f);
                        break;
                    case Rarity.Rare:
                        rarityBG.color = new Color(0f, 0f, 1f, 10f / 255f);
                        break;
                    case Rarity.Epic:
                        rarityBG.color = new Color(1, 0, 1, 10f / 255f);
                        break;
                    case Rarity.Legendary:
                        rarityBG.color = new Color(1, 1, 0, 10f / 255f);
                        break;
                    default:
                        break;
                }

                TextMeshProUGUI quantityText = materialIconInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = material.quantity.ToString(); // 수량 설정
                }
            }
        }
        else // 모든 슬롯을 비활성화
        {
            foreach (var slot in slots)
            {
                foreach (Transform child in slot.transform)
                {
                    Destroy(child.gameObject);
                }
                slot.SetActive(false);
            }
        }
    }
}