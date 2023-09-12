using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public partial class EnhancementsListController
{
    ListView _list;

    public EnhancementsListController(UIDocument document)
    {
        _list = document.rootVisualElement.Q<ListView>("Enhancements");
        CreateList();
    }

    void OnListSelected(IEnumerable<object> selectedItems)
    {
        //GunDefinition gun = _list.selectedItem as GunDefinition;
    }

    void CreateList()
    {
        _list.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Auto;
        _list.makeItem = () => {
            TemplateContainer instantiatedTemplate = BuildUI.Instance.enhancementItemTemplate.Instantiate();
            VisualElement item = instantiatedTemplate.Q<VisualElement>("Enhancement");
            item.userData = new EnhancementItemController(item);
            return item;
        };

        _list.bindItem = (item, index) =>
        {
            EnhancementItemController itemController = item.userData as EnhancementItemController;
            EnhancementDefinition definition = _list.itemsSource[index] as EnhancementDefinition;
            itemController.SetItem(definition);
        };
        _list.selectionChanged += OnListSelected;
        _list.itemsSource = GameManager.Instance.Enhancements;
    }

    public void Show(bool show)
    {
        _list.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
