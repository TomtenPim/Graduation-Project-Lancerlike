using Actions;
using Game;
using UnityEngine;

public class ThermalRifle: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Thermal Rifle";
        range = 5;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Main;
        description = "  Main\n1d3+1 Damage \nRange 5 \nArmor-Piercing";
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
        target.TakeDamage(UnityEngine.Random.Range(1, 3)+2 +target.Armor);
    }
    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
