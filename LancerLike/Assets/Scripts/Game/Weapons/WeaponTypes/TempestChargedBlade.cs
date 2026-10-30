using System.Runtime.InteropServices;
using UnityEngine;

public class TempestChargedBlade : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Tempest Charged Blade";
        range = 2;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.SuperHeavy;
        description = "  Superheavy \n3d6+4 Damage \nRange 2 \nArmor-Piercing, Knockback 2";
    }

    public override void OnHitRoll(BattlefieldObject target)
    {
        if (InvsibilityCheck(target))
        {
            base.OnMiss(target);
            return;
        }

        if (HitRollAgainstEvasion(target,0,0))
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6) + 4 + target.Armor);
        target.TakeKnockback(2, this.GetComponent<Mech>().CurrentPrimaryTile);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
