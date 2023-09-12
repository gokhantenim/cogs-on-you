using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GunSlotUIController
{
    VisualElement _slotOperations;
    VisualElement _selectedGunContainer;
    ListView _gunsList;

    Label _gunName;

    Label _damage;
    Label _fireRate;
    Label _magazineCapacity;
    Label _reloadTime;

    Label _damageNew;
    Label _fireRateNew;
    Label _magazineCapacityNew;
    Label _reloadTimeNew;

    Label _cost;

    Button _backButton;
    Button _installButton;
    Button _upgradeButton;
    Label _noUpgradeLabel;

    public GunSlotUIController(UIDocument document)
    {
        _slotOperations = document.rootVisualElement.Q("SlotOperations");
        _selectedGunContainer = document.rootVisualElement.Q("SelectedGun");
        _gunsList = document.rootVisualElement.Q<ListView>("GunsList");
        CreateGunsList();

        _gunName = _selectedGunContainer.Q<Label>("GunName");

        _damage = _selectedGunContainer.Q<Label>("Damage");
        _fireRate = _selectedGunContainer.Q<Label>("FireRate");
        _magazineCapacity = _selectedGunContainer.Q<Label>("MagazineCapacity");
        _reloadTime = _selectedGunContainer.Q<Label>("ReloadTime");

        _damageNew = _selectedGunContainer.Q<Label>("DamageNew");
        _fireRateNew = _selectedGunContainer.Q<Label>("FireRateNew");
        _magazineCapacityNew = _selectedGunContainer.Q<Label>("MagazineCapacityNew");
        _reloadTimeNew = _selectedGunContainer.Q<Label>("ReloadTimeNew");

        _cost = _selectedGunContainer.Q<Label>("Cost");

        _backButton = document.rootVisualElement.Q<Button>("BackButton");
        _installButton = document.rootVisualElement.Q<Button>("InstallButton");
        _upgradeButton = document.rootVisualElement.Q<Button>("UpgradeButton");
        _noUpgradeLabel = document.rootVisualElement.Q<Label>("NoUpgrade");

        _backButton.clicked += BuildUI.Instance.BackFromSlot;
        _installButton.clicked += BuildUI.Instance.InstallGun;
        _upgradeButton.clicked += BuildUI.Instance.UpgradeGun;
    }

    public void Refresh()
    {
        _gunsList.RefreshItems();
    }

    ~GunSlotUIController()
    {
        _backButton.clicked -= BuildUI.Instance.BackFromSlot;
        _installButton.clicked -= BuildUI.Instance.InstallGun;
        _upgradeButton.clicked -= BuildUI.Instance.UpgradeGun;
    }

    public void SetCurrentGun(Gun gun)
    {
        if(gun == null)
        {
            _damage.text = _fireRate.text = _magazineCapacity.text = _reloadTime.text = "0";
            _gunsList.selectedIndex = -1;
            return;
        }
        _damage.text = gun.damage.ToString("N0");
        _fireRate.text = gun.fireRate.ToString("N0");
        _magazineCapacity.text = gun.magazineCapacity.ToString("N0");
        _reloadTime.text = gun.reloadTime.ToString("N2");

        _gunsList.selectedIndex = GameManager.Instance.Guns.IndexOf(gun.Definition);
    }

    public void SetSelectedGun(GunSelectionItem gunItem)
    {
        ShowSelectedGun(gunItem != null);
        if(gunItem == null) return;

        _gunName.text = gunItem.Gun.Name;
        ShowButtons(gunItem.isInstall, gunItem.isUpgrade);

        _damageNew.text = gunItem.Damage;
        _fireRateNew.text = gunItem.FireRate;
        _magazineCapacityNew.text = gunItem.MagazineCapacity;
        _reloadTimeNew.text = gunItem.ReloadTime;
        _cost.text = gunItem.Cost;
    }

    void ShowButtons(bool install, bool upgrade)
    {
        _installButton.style.display = install ? DisplayStyle.Flex : DisplayStyle.None;
        _upgradeButton.style.display = upgrade ? DisplayStyle.Flex : DisplayStyle.None;
        _noUpgradeLabel.style.display = !install && !upgrade ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void ShowSelectedGun(bool show)
    {
        _selectedGunContainer.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void Show(bool show)
    {
        _slotOperations.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void OnGunListSelected(IEnumerable<object> selectedItems)
    {
        GunDefinition gun = _gunsList.selectedItem as GunDefinition;
        BuildUI.Instance.SelectGun(gun);
        SoundManager.Instance.PlayButtonSound();
    }

    void CreateGunsList()
    {
        _gunsList.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Auto;
        _gunsList.makeItem = () => {
            TemplateContainer instantiatedTemplate = BuildUI.Instance.gunCardTemplate.Instantiate();
            VisualElement gunCard = instantiatedTemplate.Q<VisualElement>("Gun");
            gunCard.userData = new GunCardUIController(gunCard);
            return gunCard;
        };

        _gunsList.bindItem = (item, index) =>
        {
            GunCardUIController card = item.userData as GunCardUIController;
            GunDefinition gun = _gunsList.itemsSource[index] as GunDefinition;
            card.SetGun(new GunSelectionItem(gun, BuildUI.Instance.SelectedSlot.CurrentGun));
        };
        _gunsList.selectionChanged += OnGunListSelected;
        //List<GunSelectionItem> gunItems = GameManager.Instance.Guns.ConvertAll(gun => new GunSelectionItem(gun));
        _gunsList.itemsSource = GameManager.Instance.Guns;
    }
}
