using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnhancementItemController
{
    VisualElement _card;
    VisualElement _image;
    Label _name;
    Label _cost;
    Button _buyButton;
    EnhancementDefinition _definition;

    public EnhancementItemController(VisualElement card)
    {
        _card = card;
        _image = _card.Q<VisualElement>("Image");
        _name = _card.Q<Label>("Name");
        _cost = _card.Q<Label>("Cost");
        _buyButton = _card.Q<Button>("BuyButton");

        _buyButton.clicked += ClickedBuy;
    }

    public void ClickedBuy()
    {
        _definition.Buy();
    }

    public void SetItem(EnhancementDefinition enhancement)
    {
        _definition = enhancement;
        _name.text = enhancement.Name;
        _cost.text = enhancement.Cost.ToString("N0");
        _image.style.backgroundImage = new StyleBackground(enhancement.Image);
    }
}
