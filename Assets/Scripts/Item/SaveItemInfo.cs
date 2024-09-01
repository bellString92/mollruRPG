using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class SaveItemInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemKind item;
    public TextMeshProUGUI quantityText; // 수량을 표시하는 텍스트
    public Image CooldownImage; // 쿨타임을 표시할 이미지
    private int lastQuantity; // 마지막으로 표시된 수량

    public GameObject tooltipPrefab; // Tooltip Prefab
    private GameObject tooltipInstance; // Tooltip Instance

    private Coroutine tooltipUpdateCoroutine;
    private Coroutine cooldownCoroutine; // 쿨타임 표시를 위한 코루틴
    private ConsumItem consumItem;

    public bool coolTimeDone = false;

    public SaveItemInfo(SaveItemInfo original)
    {
        this.item = original.item;
        this.quantityText = original.quantityText;
        CooldownImage = original.CooldownImage;
        lastQuantity = original.lastQuantity;
        tooltipPrefab = original.tooltipPrefab;
        tooltipInstance = original.tooltipInstance;
        cooldownCoroutine = original.cooldownCoroutine;
        tooltipUpdateCoroutine = original.tooltipUpdateCoroutine;             
    }

    private void Start()
    {
        CooldownImage = transform.Find("CoolDown").GetComponent<Image>();
        CooldownImage.gameObject.SetActive(false);
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
   private void OnEnable()
    {
        if (item is ConsumItem consumItem)
        {
            this.consumItem = consumItem;

            // 쿨타임이 끝났는지 확인하고 UI를 업데이트
            float remainingCooldown = CoolTimeManager.GetRemainingCooldown(consumItem.itemID);
            if (remainingCooldown > 0)
            {
                StartCooldownCoroutine(consumItem, remainingCooldown);
            }
            else
            {
                ResetCooldownUI(); // 쿨타임이 끝난 상태로 UI 갱신
            }
        }
    }

    private void ResetCooldownUI()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        // 쿨타임 UI를 초기화 (쿨타임이 끝난 상태로 표시)
        if (CooldownImage != null)
            CooldownImage.fillAmount = 0;
    }

    public void StartCooldownCoroutine(ConsumItem consumItem, float remainingCooldown = -1f)
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        if (remainingCooldown < 0)
        {
            remainingCooldown = consumItem.EffectCoolTime;
        }

        cooldownCoroutine = StartCoroutine(DisplayCooldown(consumItem, remainingCooldown));
    }

    private IEnumerator DisplayCooldown(ConsumItem consumItem, float duration)
    {
        float timeLeft = duration;
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            // 쿨타임 UI 업데이트
            CooldownImage.fillAmount = timeLeft / duration;

            yield return null;
        }

        ResetCooldownUI(); // 쿨타임이 종료된 후 UI를 초기화
    }


    // 아이템의 고유 ID를 저장하는 사전 (Dictionary)
    private static Dictionary<string, float> itemCooldowns = new Dictionary<string, float>();
    public void UseItem(Player player)
    {
        if (item is ConsumItem consumItem)
        {
            // 아이템의 고유 ID를 가져옴
            string itemId = consumItem.itemID.ToString();

            // 해당 아이템의 쿨타임이 진행 중이라면 아이템 사용을 막음
            if (itemCooldowns.ContainsKey(itemId) && Time.time - itemCooldowns[itemId] < consumItem.EffectCoolTime)
            {
                Debug.Log("아이템이 아직 쿨타임 중입니다.");
                return;
            }

            // 쿨타임 갱신
            itemCooldowns[itemId] = Time.time;

            // 쿨타임 표시 시작
            if (cooldownCoroutine != null)
            {
                StopCoroutine(cooldownCoroutine);
            }
            cooldownCoroutine = StartCoroutine(DisplayCooldown(consumItem));


            // 쿨타임이 모든 인스턴스에 적용되도록 이벤트나 브로드캐스트 호출
            ApplyCooldownToAllInstances(itemId);
        }

        item.Use(player);
    }

    // 모든 동일한 ID의 아이템 인스턴스에 쿨타임 적용
    private IEnumerator DisplayCooldown(ConsumItem consumItem)
    {
        // 쿨타임 시작 시 이미지를 활성화
        coolTimeDone = false;
        CooldownImage.gameObject.SetActive(true);

        float startTime = itemCooldowns[consumItem.itemID.ToString()];  // 모든 인스턴스가 동일한 쿨타임을 참조하도록 함.

        while (Time.time - startTime < consumItem.EffectCoolTime)
        {
            float timePassed = Time.time - startTime;
            float cooldownRatio = (consumItem.EffectCoolTime - timePassed) / consumItem.EffectCoolTime;

            // 이미지의 fillAmount를 사용하여 쿨타임 표시
            CooldownImage.fillAmount = cooldownRatio;

            yield return null;
        }

        // 쿨타임 종료 시 이미지를 비활성화
        CooldownImage.fillAmount = 0f; // 쿨타임 종료 시 이미지를 초기화
        CooldownImage.gameObject.SetActive(false);
        coolTimeDone = true;
    }

    public void StartCooldownCoroutine()
    {
        // 코루틴이 이미 실행 중인 경우 중지
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        // `item`이 `ConsumItem`일 때만 쿨타임 시작
        if (item is ConsumItem consumItem)
        {
            // 모든 인스턴스에서 같은 쿨타임을 참조
            cooldownCoroutine = StartCoroutine(DisplayCooldown(consumItem));
        }
    }

    private void ApplyCooldownToAllInstances(string itemId)
    {
        SaveItemInfo[] allItems = FindObjectsOfType<SaveItemInfo>();
        foreach (var itemInfo in allItems)
        {
            if (itemInfo.item.itemID.ToString() == itemId)
            {
                itemInfo.StartCooldownCoroutine();
            }
        }
    }

    // 아이템 툴팁 표시

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
        if(item.quantity <=0)
        {
            Destroy(this.gameObject);
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

