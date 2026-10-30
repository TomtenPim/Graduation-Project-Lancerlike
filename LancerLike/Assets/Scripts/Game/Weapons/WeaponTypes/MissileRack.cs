using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class MissileRack: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Missile Rack";
        range = 10;
        isBlast = true;
        blastRange = 1;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Aux;
        description = "  Auxiliary \n1d3+1 Damage \nRange 10, Blast 1\nLoading";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 3)+ 1);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
