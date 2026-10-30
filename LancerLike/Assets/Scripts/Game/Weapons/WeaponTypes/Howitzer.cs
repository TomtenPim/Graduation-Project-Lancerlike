using UnityEngine;

public class Howitzer : Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Howitzer";
        range = 20;
        isBlast = true;
        blastRange = 2;
        ignoresLineOfSight = true;
        isOrdnance = true;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Heavy;
        description = "  Heavy \n2d6 Damage \nRange 20, Blast 2\nArcing, Inaccurate 1, Loading, Ordnance";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+ Mathf.Max(UnityEngine.Random.Range(1, 6)));
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
