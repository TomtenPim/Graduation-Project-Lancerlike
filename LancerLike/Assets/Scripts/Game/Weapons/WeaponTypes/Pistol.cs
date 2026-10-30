using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Pistol : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Pistol";
        range = 5;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Aux;
        description = "  Auxiliary \n1d3 Damage \nRange 5\nReliable 1";
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
        target.TakeDamage(Mathf.Max(UnityEngine.Random.Range(1, 3),1));
    }

    public override void OnMiss(BattlefieldObject target)
    {
        target.TakeDamage(1);
    }
}
