using UnityEngine;

public class CyclonePulseRifle : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Cyclone Pulse Rifle";
        range = 15;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.SuperHeavy;
        description = "  Superheavy \n3d6+3 Damage \nRange 15 \nAccurate 1, Loading, Reliable 5";
    }
    public override void OnAttack()
    {
        canShootAgainWithoutLoading = false;
    }
    public override void OnHitRoll(BattlefieldObject target)
    {
        if (InvsibilityCheck(target))
        {
            base.OnMiss(target);
            return;
        }

        if (HitRollAgainstEvasion(target,1,0))
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6) + 3);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        target.TakeDamage(5);
    }
}
