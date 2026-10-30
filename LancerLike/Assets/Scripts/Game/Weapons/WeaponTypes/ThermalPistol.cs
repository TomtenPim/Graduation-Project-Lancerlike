using Actions;
using Game;
using UnityEngine;

public class ThermalPistol: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Thermal Pistol";
        range = 5;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Aux;
        description = "  Auxiliary\n2 Damage \nLine: 5";
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
        target.TakeDamage(2);
    }
    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
