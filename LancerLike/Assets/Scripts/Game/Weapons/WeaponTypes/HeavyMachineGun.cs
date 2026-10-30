using UnityEngine;

public class HeavyMachineGun: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Heavy Machine Gun";
        range = 8;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Heavy;
        description = "  Heavy \n2d6+4 Damage \nRange 8 \nInaccurate 1";
    }

    public override void OnHitRoll(BattlefieldObject target)
    {
        if (InvsibilityCheck(target))
        {
            base.OnMiss(target);
            return;
        }

        if (HitRollAgainstEvasion(target, 0, 1))
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ UnityEngine.Random.Range(1, 6)+4);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
