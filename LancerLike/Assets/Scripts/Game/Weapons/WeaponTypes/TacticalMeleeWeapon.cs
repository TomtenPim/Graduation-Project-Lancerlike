using UnityEngine;

public class TacticalMeleeWeapon : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Tactical Melee Weapon";
        range = 1;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Main;
        description = "  Main \n1d6+2 Damage \nRange 1";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+2);
    }
    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
