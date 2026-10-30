using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class RocketPropelledGrenade: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Rocket-Propelled Grenade";
        range = 10;
        isBlast = true;
        blastRange = 2;
        isOrdnance = true;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Main;
        description = "  Main \n1d6+1 Damage \nRange 10, Blast 2\nLoading, Ordnance";
    }
    public override void OnAttack()
    {
        base.OnAttack();
        canShootAgainWithoutLoading = false;
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ 1);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
