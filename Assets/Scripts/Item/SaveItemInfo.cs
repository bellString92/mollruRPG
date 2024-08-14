using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;

public class SaveItemInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemKind item;
    public TextMeshProUGUI quantityText; // 수량을 표시하는 텍스트
    private int lastQuantity; // 마지막으로 표시된 수량

    public GameObject tooltipPrefab; // Tooltip Prefab
    private GameObject tooltipInstance; // Tooltip Instance

    private Coroutine tooltipUpdateCoroutine;


    private void Start()
    {
        quantityText = GetComponentInChildren<TextMeshProUGUI>();
        if (quantityText != null)
        {
            lastQuantity = item.quantity;
            quantityText.fontSize = 18.0f;
            UpdateQuantityText(); // 초기 수량 업데이트
        }
        tooltipPrefab = Resources.Load<GameObject>("Prefabs/Tooltip");
    }

    private void Update()
    {
        // 수량이 변경되었는지 확인하고 UI를 업데이트합니다.
        if (item.itemType == ItemType.consumItem || item.itemType == ItemType.materialItem)
        {
            if (item.quantity != lastQuantity)
            {
                UpdateQuantityText();
                lastQuantity = item.quantity;
            }
        }
        if (!transform.gameObject.activeInHierarchy)
        {
            CloseTooltip();
        }
    }
    private void OnDisable()
    {
        // 오브젝트가 비활성화될 때 툴팁을 닫습니다.
        CloseTooltip();
    }

    private void UpdateQuantityText()
    {
        if (quantityText != null)
        {
            if (item.quantity == 1)
            {
                quantityText.text = null;
            }
            else
                quantityText.text = item.quantity.ToString();
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Tooltip 생성 및 위치 설정
        if (tooltipPrefab != null)
        {
            // 기존의 Tooltip이 있는 경우 삭제
            if (tooltipInstance != null)
            {
                Destroy(tooltipInstance);
            }

            // Tooltip을 현재 오브젝트의 자식으로 생성
            tooltipInstance = Instantiate(tooltipPrefab, UIManager.Instance.transform);
            // RectTransform을 사용하여 위치 및 앵커 설정
            RectTransform tooltipRect = tooltipInstance.GetComponent<RectTransform>();

            // 기본 앵커를 좌상단으로 설정
            SetTooltipAnchor(tooltipRect, AnchorPosition.TopLeft);

            // 부모 자식 관계에서 제일 위로 배치
            tooltipInstance.transform.SetAsLastSibling();

            // 위치를 현재 오브젝트의 위치로 설정
            Vector3 worldPosition = transform.position;
            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            tooltipRect.position = transform.position;

            // Tooltip에 정보 설정
            var tooltip = tooltipInstance.GetComponent<Tooltip>();
            // 툴팁 업데이트 코루틴 시작
            tooltipUpdateCoroutine = StartCoroutine(UpdateTooltipCoroutine(tooltip));

            // 화면 경계를 벗어나는지 확인하고 앵커 조정
            AdjustTooltipAnchor(tooltipRect);
            tooltipRect.position = transform.position;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CloseTooltip();
    }
    private void CloseTooltip()
    {
        // Tooltip 제거
        if (tooltipInstance != null)
        {
            Destroy(tooltipInstance);
        }
        // 툴팁 업데이트 코루틴 중지
        if (tooltipUpdateCoroutine != null)
        {
            StopCoroutine(tooltipUpdateCoroutine);
            tooltipUpdateCoroutine = null;
        }
    }
    private IEnumerator UpdateTooltipCoroutine(Tooltip tooltip)
    {
        while (true)
        {
            if (tooltip != null)
            {
                tooltip.SetText(GetItemInfo());
            }
            yield return new WaitForSeconds(0.5f); // 0.5초마다 업데이트
        }
    }
    private enum AnchorPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    // 생성되는 정보창이 화면밖으로 나가는 걸 방지
    private void SetTooltipAnchor(RectTransform tooltipRect, AnchorPosition anchorPosition)
    {
        switch (anchorPosition)
        {
            case AnchorPosition.TopLeft:
                tooltipRect.anchorMin = new Vector2(0, 1);
                tooltipRect.anchorMax = new Vector2(0, 1);
                tooltipRect.pivot = new Vector2(0, 1);
                break;
            case AnchorPosition.TopRight:
                tooltipRect.anchorMin = new Vector2(1, 1);
                tooltipRect.anchorMax = new Vector2(1, 1);
                tooltipRect.pivot = new Vector2(1, 1);
                break;
            case AnchorPosition.BottomLeft:
                tooltipRect.anchorMin = new Vector2(0, 0);
                tooltipRect.anchorMax = new Vector2(0, 0);
                tooltipRect.pivot = new Vector2(0, 0);
                break;
            case AnchorPosition.BottomRight:
                tooltipRect.anchorMin = new Vector2(1, 0);
                tooltipRect.anchorMax = new Vector2(1, 0);
                tooltipRect.pivot = new Vector2(1, 0);
                break;
        }
    }
    private void AdjustTooltipAnchor(RectTransform tooltipRect)
    {
        Canvas canvas = UIManager.Instance.GetComponent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        Vector3[] tooltipCorners = new Vector3[4];
        tooltipRect.GetWorldCorners(tooltipCorners);

        bool isOutOfRight = tooltipCorners[2].x > canvasCorners[2].x;
        bool isOutOfBottom = tooltipCorners[0].y < canvasCorners[0].y;

        if (isOutOfRight && isOutOfBottom)
        {
            SetTooltipAnchor(tooltipRect, AnchorPosition.BottomRight);
        }
        else if (isOutOfRight)
        {
            SetTooltipAnchor(tooltipRect, AnchorPosition.TopRight);
        }
        else if (isOutOfBottom)
        {
            SetTooltipAnchor(tooltipRect, AnchorPosition.BottomLeft);
        }
    }


    private string GetItemInfo()
    {
        if (item == null)
        {
            Debug.LogError("ItemKind is not of type ItemInfo.");
            return string.Empty;
        }
        string name;
        if (item.kaiLevel != 0)
        {
            name = $"[ {item.itemName} <color=yellow>+{item.kaiLevel}</color> ]\n";
        }
        else
        {
            name = $"[ {item.itemName} ]\n";
        }

        Color rareityColor;
        string rarityColorHex = "";
        switch (item.rarity)
        {
            case Rarity.Common:
                rareityColor = Color.white;
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Uncommon:
                rareityColor = new Color(0, 1, 0);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Rare:
                rareityColor = new Color(0.3f, 0.3f, 1f);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Epic:
                rareityColor = new Color(1, 0, 1);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
            case Rarity.Legendary:
                rareityColor = new Color(1, 1, 0);
                rarityColorHex = ColorUtility.ToHtmlStringRGB(rareityColor);
                break;
        }

        // 기본 정보 설정
        string info = $"<b>{name}</b>\n" +
                      $"등급 : <b><color=#{rarityColorHex}>{item.rarity}</color></b>\n" +
                      $"분류 : {item.itemType}\n";

        // 추가적인 정보 표시를 위해 switch문 사용
        switch (item.itemType)
        {
            case ItemType.weaponItem:
                info += DisplayWeaponInfo(item as WeaponItem);
                break;
            case ItemType.armorItem:
                info += DisplayArmorInfo(item as ArmorItem);
                break;
            case ItemType.acceItem:
                info += DisplayAccessoryInfo(item as AcceItem);
                break;
            case ItemType.consumItem:
                info += DisplayConsumableInfo(item as ConsumItem);
                break;
            case ItemType.materialItem:
                info += DisplayMaterialInfo(item as MaterialItem);
                break;
            default:
                Debug.LogWarning("Unknown ItemType: " + item.itemType);
                break;
        }

        // 공통 정보 추가
        info += $"[ {item.description} ]\n" +
                $"판매 가격 : <color=yellow>{item.resellprice}({item.resellprice * item.quantity})</color>";

        return info;
    }
    private string DisplayWeaponInfo(WeaponItem weaponInfo)
    {
        string info = string.Empty;
        if (weaponInfo != null)
        {
            // 기본적으로 모든 무기가 가지는 공격력 보너스 표시
            info += $"<b>무기 공격력 : {weaponInfo.CalculateEffectiveAttackBoost()}</b>\n";

            // 효과 목록을 순회하여 설정된 효과만 표시
            foreach (var effect in weaponInfo.effectList)
            {
                switch (effect.effectType)
                {
                    case WeaponEffectType.MaxHealthBoost:
                        info += $"<b>추가 체력 : {effect.effectValue}</b>\n";
                        break;
                    case WeaponEffectType.CritChanceBoost:
                        info += $"<b>치명타 확률 : {effect.effectValue}</b>\n";
                        break;
                    case WeaponEffectType.CritDamageBoost:
                        info += $"<b>치명타 대미지 : {effect.effectValue}</b>\n";
                        break;
                    case WeaponEffectType.SpeedBoost:
                        info += $"<b>추가 속도 : {effect.effectValue}</b>\n";
                        break;
                    // 추가적인 효과 
                    default:
                        info += $"<b>Unknown Effect Type: {effect.effectType}</b>\n";
                        break;
                }
            }
        }
        return info;
    }

    private string DisplayArmorInfo(ArmorItem armorInfo)
    {
        string info = string.Empty;
        if (armorInfo != null)
        {
            // 방어구 부위 정보 표시
            info += $"부위: {armorInfo.armorType}\n";
            // 기본적으로 모든 무기가 가지는 공격력 보너스 표시
            info += $"<b>방어구 체력 : {armorInfo.CalculateEffectiveMaxHealBoost()}</b>\n";

            // 효과 목록을 순회하여 설정된 효과만 표시
            foreach (var effect in armorInfo.effectList)
            {
                switch (effect.effectType)
                {
                    case ArmorEffectType.AttackBoost:
                        info += $"<b>추가 공격력: {effect.effectValue}</b>\n";
                        break;
                    case ArmorEffectType.CritChanceBoost:
                        info += $"<b>치명타 확률 : {effect.effectValue}</b>\n";
                        break;
                    case ArmorEffectType.CritDamageBoost:
                        info += $"<b>치명타 대미지 : {effect.effectValue}</b>\n";
                        break;
                    case ArmorEffectType.SpeedBoost:
                        info += $"<b>추가 속도 : {effect.effectValue}</b>\n";
                        break;
                    // 추가적인 효과 
                    default:
                        info += $"<b>Unknown Effect Type: {effect.effectType}</b>\n";
                        break;
                }
            }

        }
        return info;
    }

    private string DisplayAccessoryInfo(AcceItem acceInfo)
    {
        string info = string.Empty;
        if (acceInfo != null)
        {
            // 방어구 부위 정보 표시
            info += $"부위: {acceInfo.AcceType}\n";
            foreach (var effect in acceInfo.effectList)
            {
                switch (effect.effectType)
                {
                    case AcceEffectType.AttackBoost:
                        info += $"<b>추가 공격력: {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.MaxHealthBoost:
                        info += $"<b>추가 체력: {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.CritChanceBoost:
                        info += $"<b>치명타 확률 : {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.CritDamageBoost:
                        info += $"<b>치명타 대미지 : {effect.effectValue}</b>\n";
                        break;
                    case AcceEffectType.SpeedBoost:
                        info += $"<b>추가 속도 : {effect.effectValue}</b>\n";
                        break;
                    // 추가적인 효과 
                    default:
                        info += $"<b>Unknown Effect Type: {effect.effectType}</b>\n";
                        break;
                }
            }
        }
        return info;
    }

    private string DisplayConsumableInfo(ConsumItem consumInfo)
    {
        string info = string.Empty;
        info += $"소지수 : {item.quantity}\n";
        if (consumInfo != null)
        {
            switch(consumInfo.effectType)
            {
                case EffectType.HealEffect:
                    info += $"<b>회복량 : {consumInfo.EffectPoint}</b>\n";
                    break;
                case EffectType.AttackBoostEffect:
                    info += $"<b>공격력 증가 : {consumInfo.EffectPoint}</b>\n";
                    info += $"<b>지속 시간 : {consumInfo.EffectDuration}</b>\n";
                    break;
                case EffectType.SpeedBoostEffect:
                    info += $"<b>속도 증가 : {consumInfo.EffectPoint}</b>\n";
                    info += $"<b>지속 시간 : {consumInfo.EffectDuration}</b>\n";
                    break;
            }
        }
        return info;
    }

    private string DisplayMaterialInfo(MaterialItem MaterialInfo)
    {
        string info = string.Empty;
        info += $"소지수 : {item.quantity}\n";
        return info;
    }
}

