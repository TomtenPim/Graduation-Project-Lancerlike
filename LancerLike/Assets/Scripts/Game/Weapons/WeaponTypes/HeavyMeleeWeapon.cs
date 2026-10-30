using UnityEngine;

public class HeavyMeleeWeapon : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Heavy Melee Weapon";
        range = 1;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Heavy;
        description = "  Heavy \n2d6+1 Damage \nRange 1";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ UnityEngine.Random.Range(1, 6)+1);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
