using UnityEngine;

public class AntiMaterielRifle: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Anti-Materiel Rifle";
        range = 20;
        isOrdnance = true;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Heavy;
        description = "  Heavy \n2d6 Damage \nRange 20 \nAccurate 1, Armor-Piercing, Loading, Ordnance";
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

        if (HitRollAgainstEvasion(target, 1, 0))
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
        target.TakeDamage(Mathf.Max(UnityEngine.Random.Range(1, 6)+ Mathf.Max(UnityEngine.Random.Range(1, 6) + target.Armor)));
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
