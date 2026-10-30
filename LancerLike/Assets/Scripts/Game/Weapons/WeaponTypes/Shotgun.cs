using UnityEngine;

public class Shotgun : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Shotgun";
        range = 5;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Main;
        description = "  Main \n1d6 Damage \nRange 5";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6));
    }
    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
