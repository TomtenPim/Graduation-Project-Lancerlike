using Actions;
using Game;
using UnityEngine;

public class ThermalLance: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Thermal Lance";
        range = 10;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Heavy;
        description = "  Heavy \n1d6+3 Damage \nLine: 10\nHeat 2 (self)";
    }
    public override void OnAttack()
    {
        base.OnAttack();
        GetComponent<Mech>().TakeHeat(2);
    }

    public override void OnSelectTargets()
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerLineAttackAction plaa = PlayerLineAttackAction.Create(canvas.transform, this);
        ActionStack.Main.PushAction(plaa);
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
        target.TakeDamage(UnityEngine.Random.Range(1, 6)+3);
    }
    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
