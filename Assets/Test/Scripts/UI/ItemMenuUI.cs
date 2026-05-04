using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenuUI : MonoBehaviour
{
    public GameObject panel;
    private Unit currentUnit;
    private int selectedIndex = -1;
    private UnitController controller;
    public ItemActionMenuUI actionMenu;
    
    public List<ItemSlot> itemSlots;
    void Start()
    {
        controller = FindFirstObjectByType<UnitController>();  
    }
    
    public void Show(Unit unit)
    {
        panel.SetActive(true);
        
        panel.transform.localScale = Vector3.zero;
        panel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        currentUnit = unit;
        selectedIndex = -1;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);
        panel.transform.position = screenPos;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < unit.inventory.Count)
            {
                Item item = unit.inventory[i];

                itemSlots[i].SetItem(item.itemName);

                int index = i;

                itemSlots[i].button.onClick.RemoveAllListeners();
                itemSlots[i].button.onClick.AddListener(() => SelectItem(index));
            }
            else
            {
                itemSlots[i].SetEmpty();
            }
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }


    void SelectItem(int index)
    {
        selectedIndex = index;

        Debug.Log("Chọn item: " + currentUnit.inventory[index].itemName);

        // 🔥 lấy vị trí panel item
        RectTransform rt = panel.GetComponent<RectTransform>();

        // 🔥 tính vị trí bên phải menu item
        Vector3 pos = rt.position + new Vector3(rt.rect.width, 0, 0);
        // 🔥 lấy vị trí panel trên màn hình (0 → 1)
        if (rt.position.x > Screen.width / 2f)
        {
            pos.x -= 230f;
        }
        actionMenu.Show(index, pos);
    }
    
}