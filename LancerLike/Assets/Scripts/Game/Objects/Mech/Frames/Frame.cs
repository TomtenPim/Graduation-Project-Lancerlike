using Actions;
using Game;
using System.Collections.Generic;
using UnityEngine;

public class Frame : MonoBehaviour
{
    public BattlefieldObject.Size objectSize;
    public string objectName;

    public int structure;
    public int hp;
    public int stress;
    public int heatCapacity;

    public int armor;
    public int evasion;
    public int eDefence;

    public int sensors;
    public int techAttack;

    public int repairCapacity;
    public int saveTarget;

    public int speed;
    public int systemPoints;
    public List <Mech.WeaponMount> weaponMounts = new();

    public virtual void CoreAbility()
    {

    }
}

