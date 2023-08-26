using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepairEnhancement", menuName = "Create Repair Enhancement")]
public class RepairEnhancement : EnhancementDefinition
{
    public override void Buy()
    {
        Cogs cogs = PlayerController.Instance.GetComponent<Cogs>();
        Damagable damagable = PlayerController.Instance.GetComponent<Damagable>();
        if (cogs.TotalCogs < Cost)
        {
            UIManager.Instance.Modal.Alert("Insufficient amount of cogs");
            return;
        }
        if(damagable.HealthPercent >= 1)
        {
            UIManager.Instance.Modal.Alert("Your health is already full");
            return;
        }
        SoundManager.Instance.PlayBuildSound();
        cogs.Spend(Cost);
        damagable.ResetHealth();
    }
}
