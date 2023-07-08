using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GunCardUIController
{
    VisualElement _card;
    Label _name;
    Label _damage;
    Label _fireRate;
    Label _magazineCapacity;
    Label _reloadTime;
    Label _cost;

    public GunCardUIController(VisualElement card)
    {
        _card = card;
        _name = _card.Q<Label>("Name");
        _damage = _card.Q<Label>("Damage");
        _fireRate = _card.Q<Label>("FireRate");
        _magazineCapacity = _card.Q<Label>("MagazineCapacity");
        _reloadTime = _card.Q<Label>("ReloadTime");
        _cost = _card.Q<Label>("Cost");
    }

    public void SetGun(GunSelectionItem gunItem)
    {
        _name.text = gunItem.Gun.Name;
        _damage.text = gunItem.Damage;
        _fireRate.text = gunItem.FireRate;
        _magazineCapacity.text = gunItem.MagazineCapacity;
        _reloadTime.text = gunItem.ReloadTime;
        _cost.text = gunItem.Cost;
    }
}
