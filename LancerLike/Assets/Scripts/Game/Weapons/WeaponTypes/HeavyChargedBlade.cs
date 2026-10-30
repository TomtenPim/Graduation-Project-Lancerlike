using System.Runtime.InteropServices;
using UnityEngine;

public class HeavyChargedBlade : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Heavy Charged Blade";
        range = 1;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Heavy;
        description = "  Heavy \n1d6+3 Damage \nRange 1 \nArmor-Piercing";
    }

    public override void OnHitRoll(BattlefieldObject target)
    {
        if (InvsibilityCheck(target))
        {
            base.OnMiss(target);
            return;
        }

        if (HitRollAgainstEvasion(target, 0, 0))
        {
            OnHit(target);
        }
        else
        {
            OnMiss(target);
        }
    }

    public override void OnHit(BattlefieldObject target)
    {
        target.TakeDamage(UnityEngine.Random.Range(1, 6) + 3 + target.Armor);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
