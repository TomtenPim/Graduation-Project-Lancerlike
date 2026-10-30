using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class HurricaneClusterProjector : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Hurricane Cluster Projector";
        range = 10;
        isBlast = true;
        blastRange = 2;
        ignoresLineOfSight = true;
        isOrdnance = true;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.SuperHeavy;
        description = "  Superheavy \n1d6+6 Damage \nRange 10, Blast 2\nHeat 2 (Self), Ordnance, Seeking, Smart";
    }
    public override void OnAttack()
    {
        base.OnAttack();
        GetComponent<Mech>().TakeHeat(2);
    }
    public override void OnHitRoll(BattlefieldObject target)
    {
        if (InvsibilityCheck(target))
        {
            base.OnMiss(target);
            return;
        }

        if (HitRollAgainstEDefence(target,0,0))
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ 6);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
