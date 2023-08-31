using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GunSlotsUIController
{
    VisualElement _slotsContainer;
    List<SlotUI> slots = new List<SlotUI>();
    Camera _camera;

    public class SlotUI
    {
        public GunSlot slot;
        public VisualElement slotButton;
    }

    public GunSlotsUIController(UIDocument document, Camera camera)
    {
        _camera = camera;
        _slotsContainer = document.rootVisualElement.Q("Slots");
    }

    public void CreateSlotButtons()
    {
        _slotsContainer.Clear();
        slots.Clear();
        foreach (GunSlot slot in PlayerController.Instance.Character.Slots)
        {
            AddSlotButton(slot);
        }
    }

    void AddSlotButton(GunSlot slot)
    {
        TemplateContainer instantiatedTemplate = BuildUI.Instance.slotButtonTemplate.Instantiate();
        Button button = instantiatedTemplate.Q<Button>("SlotButton");
        button.clicked += () => BuildUI.Instance.SelectSlot(slot);
        slots.Add(new SlotUI()
        {
            slot = slot,
            slotButton = button
        });
        _slotsContainer.Add(button);
    }

    internal void Update()
    {
        float xRatio = Screen.width / _slotsContainer.layout.width;
        float yRatio = Screen.height / _slotsContainer.layout.height;
        foreach (SlotUI slot in slots)
        {
            Vector3 slotScreenPosition = _camera.WorldToScreenPoint(slot.slot.transform.position);
            slot.slotButton.style.left = slotScreenPosition.x / xRatio - slot.slotButton.layout.width / 2;
            slot.slotButton.style.top = _slotsContainer.layout.height - slotScreenPosition.y / yRatio - slot.slotButton.layout.height / 2;
            //slot.slotButton.style.left = 900 - 150;
            //slot.slotButton.style.top = 1600 - 150;

            //Debug.Log($"{slot.slotButton.layout.width}, {slot.slotButton.layout.height}");
        }
    }

    public void Show(bool show)
    {
        _slotsContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
