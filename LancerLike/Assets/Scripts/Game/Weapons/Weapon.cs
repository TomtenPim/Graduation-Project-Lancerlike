using Actions;
using Game;
using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[Serializable]
public class Weapon : MonoBehaviour
{
    public enum DamageType
    {
        Kinetic,
        Explosive,
        Energy
    }

    public string weaponName;
    public int range;
    public Mech.WeaponMount.WeaponMountType weaponMountTypeRequred;
    public bool ignoresLineOfSight;
    public bool canShootAgainWithoutLoading = true;
    public bool isBlast;
    public bool isOrdnance;
    public int blastRange;
    public string description;
    public bool hasfired = false;
    private Mech mechAttachedTo;

    public virtual void Start()
    {
        mechAttachedTo = GetComponent<Mech>();
    }
    public virtual void OnSelectTargets()
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerNormalAttackAction pnaa = PlayerNormalAttackAction.Create(canvas.transform, this);
        ActionStack.Main.PushAction(pnaa);
    }


    // public virtual void MakeAttack(BattlefieldObject target) { }
    public virtual void OnAttack()
    {
        hasfired = true;
    }
    public virtual void OnHitRoll(BattlefieldObject target) { }
    public virtual void OnHit(BattlefieldObject target) { }
    public virtual void OnMiss(BattlefieldObject target)
    {
        DisplayMiss(target);
    }
    private void DisplayMiss(BattlefieldObject target)
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        TextPopup.Create(canvas.transform, target.transform.position, "Miss");
    }

    protected bool InvsibilityCheck(BattlefieldObject target)
    {
        if ((1 << 4 & target.statuses) != 0) // if 4 INVISIBLE is true. 
        {
            return Convert.ToBoolean(UnityEngine.Random.Range(0, 1));
        }
        else { return false; }
    }

    public bool HitRollAgainstEvasion(BattlefieldObject target, int inAccuracy, int inDifficulty)
    {
        float totalBonusDice = inAccuracy + ((target.statuses) & 1 << 7) >> 7; // +1 accuracy if target has 7 PRONE
        totalBonusDice -= inDifficulty + ((mechAttachedTo.statuses) & 1 << 3) >> 3; // +1 difficulty if attacker has 3 IMPAIRED

        if (((~target.statuses) & 1 << 6) >> 6 == 1) // if target has 6 LOCK ON
        {
            totalBonusDice++; // +1 accuracy
            target.statuses &= ~(1 << 6); // remove 6 LOCK ON
        }

        float largestBonusRoll = 0;
        for (int i = 0; i < Mathf.Abs(totalBonusDice); i++)
        {
            largestBonusRoll = Mathf.Max(largestBonusRoll, UnityEngine.Random.Range(1, 6));
        }
        largestBonusRoll = largestBonusRoll * largestBonusRoll / MathF.Abs(largestBonusRoll);

        return UnityEngine.Random.Range(1, 20) + largestBonusRoll >= target.Evasion;
    }

    public bool HitRollAgainstEDefence(BattlefieldObject target, int inAccuracy, int inDifficulty)
    {
        float totalBonusDice = inAccuracy + ((target.statuses) & 1 << 7) >> 7; // +1 accuracy if target has 7 PRONE
        totalBonusDice -= inDifficulty + ((mechAttachedTo.statuses) & 1 << 3) >> 3; // +1 difficulty if attacker has 3 IMPAIRED

        if (((target.statuses) & 1 << 6) >> 6 == 1) // if target has 6 LOCK ON...
        {
            totalBonusDice++; // +1 accuracy
            target.statuses &= ~(1 << 6); // remove 6 LOCK ON
        }

        float largestBonusRoll = 0;
        for (int i = 0; i < Mathf.Abs(totalBonusDice); i++)
        {
            largestBonusRoll = Mathf.Max(largestBonusRoll, UnityEngine.Random.Range(1, 6));
        }
        largestBonusRoll = largestBonusRoll * largestBonusRoll / MathF.Abs(largestBonusRoll);

        if (target.EDefence == -1)
        {
            return UnityEngine.Random.Range(1, 20) + largestBonusRoll >= 8;
        }

        return UnityEngine.Random.Range(1, 20) + largestBonusRoll >= target.EDefence;
    }
}