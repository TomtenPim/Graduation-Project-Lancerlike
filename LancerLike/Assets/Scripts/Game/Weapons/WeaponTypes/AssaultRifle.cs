using UnityEngine;

public class AssaultRifle: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Assault Rifle";
        range = 10;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Main;
        description = "  Main \n1d6 Damage \nRange 10 \nReliable 2";
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
        target.TakeDamage(Mathf.Max(UnityEngine.Random.Range(1, 6), 2));
    }

    public override void OnMiss(BattlefieldObject target)
    {
        target.TakeDamage(2);
    }
}
