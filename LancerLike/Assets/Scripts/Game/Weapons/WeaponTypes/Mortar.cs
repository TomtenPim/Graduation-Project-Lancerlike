using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Mortar : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Mortar";
        range = 15;
        isBlast = true;
        ignoresLineOfSight = true;
        blastRange = 1;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Main;
        description = "  Main \n1d3+1 Damage \nRange 15, Blast 1\nArcing, Inaccurate 1";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ 1);
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
