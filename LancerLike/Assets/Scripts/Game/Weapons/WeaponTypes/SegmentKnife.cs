using UnityEngine;

public class SegmentKnife: Weapon
{
    public override void Start()
    {
        base.Start();
        weaponName = "Segment Knife";
        range = 1;
        weaponMountTypeRequred = Mech.WeaponMount.WeaponMountType.Aux;
        description = "  Auxilary \n1d3+1 Damage \nRange 1 \nOverKill 1";
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
        int damageRoll = OverchargedDamageRoll();

        target.TakeDamage(damageRoll + 1);
    }

    public int OverchargedDamageRoll()
    {
        int roll = UnityEngine.Random.Range(1, 3);

        while (roll == 1)
        {
            this.GetComponent<Mech>().TakeHeat(1);
            roll = UnityEngine.Random.Range(1, 3);
        }
        return roll;
    }

    public override void OnMiss(BattlefieldObject target)
    {
        base.OnMiss(target);
    }
}
