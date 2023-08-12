using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagneticShieldEnhancement", menuName = "Create Magnetic Shield Enhancement")]
public class MagneticShieldEnhancement : EnhancementDefinition
{
    public GameObject ShieldPrefab;
    public override void Buy()
    {
        Cogs cogs = PlayerController.Instance.GetComponent<Cogs>();
        Damagable damagable = PlayerController.Instance.GetComponent<Damagable>();
        if(damagable.Shield != null)
        {
            UIManager.Instance.Modal.Alert("You already have a shield");
            return;
        }
        if (cogs.TotalCogs < Cost)
        {
            UIManager.Instance.Modal.Alert("Insufficient amount of cogs");
            return;
        }

        GameObject shield = Instantiate(ShieldPrefab, damagable.transform);
        shield.transform.position = PlayerController.Instance.ShieldPosition.position;
        damagable.Shield = shield.GetComponent<MagneticShield>();

        cogs.Spend(Cost);
    }
}
