using UnityEngine;

public class GunSelectionItem
{
    public GunDefinition Gun;
    public bool isInstall = false;
    public bool isUpgrade = false;
    public int Level = 0;

    public string Damage;
    public string FireRate;
    public string MagazineCapacity;
    public string ReloadTime;
    public string Cost;

    public GunSelectionItem(GunDefinition gun, Gun currentGun)
    {
        Gun = gun;
        AdjustForCurrent(currentGun);
    }

    public void AdjustForCurrent(Gun currentGun)
    {
        CheckStatus(currentGun);
        SetValues();
    }

    public void CheckStatus(Gun currentGun)
    {
        if (currentGun != null && currentGun.Definition.Equals(Gun))
        {
            // upgrade
            if (currentGun.Definition.Levels.Length <= currentGun.Level + 1)
            {
                // no upgrade
                isInstall = isUpgrade = false;
                Level = currentGun.Level;
                return;
            }

            isInstall = false;
            isUpgrade = true;
            Level = currentGun.Level + 1;
            return;
        }
        // install
        isInstall = true;
        isUpgrade = false;
        Level = 0;
    }

    void SetValues()
    {
        GunDefinition.GunLevel level = Gun.Levels[Level];
        Damage = level.Damage.ToString("N0");
        FireRate = level.FireRate.ToString("N0");
        MagazineCapacity = level.MagazineCapacity.ToString("N0");
        ReloadTime = level.ReloadTime.ToString("N2");
        if (!isInstall && !isUpgrade)
        {
            Cost = "0";
            return;
        }
        Cost = level.Cost.ToString("N0");
    }
}
