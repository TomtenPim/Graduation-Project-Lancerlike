using Actions;
using Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Mech : BattlefieldObject
{
    public abstract class MechAction : ActionStack.ActionObject
    {
        protected Mech thisMech;

        public MechAction(Mech unit)
        {
            thisMech = unit;
        }

  
    }

    [Serializable]
    public class WeaponMount
    {
        public WeaponMount(WeaponMountType inMountType)
        {
            weaponMountType = inMountType;
        }
        public enum WeaponMountType
        {
            None,
            Aux,
            Main,
            Flex,
            Heavy,
            SuperHeavy
        }

        public WeaponMountType weaponMountType;
        public List<Weapon> mountedWeapons = new();

        public void AddWeaponToMount(Weapon inWeapon)
        {

            if (inWeapon.weaponMountTypeRequred > weaponMountType)
            {
                Debug.Log("This weapon is too large for the mount");
                return;
            }

            if (inWeapon.weaponMountTypeRequred == WeaponMountType.Aux && weaponMountType == WeaponMountType.Flex && mountedWeapons.Count == 1)
            {
                if (mountedWeapons[0].weaponMountTypeRequred == WeaponMountType.Aux)
                {
                    mountedWeapons.Add(inWeapon);
                }
                return;
            }

            if (mountedWeapons.Count != 0)
            {
                Debug.Log("This mount is already used");
                return;
            }

            mountedWeapons.Add(inWeapon);
        }

        public void EmptyMount()
        {
            mountedWeapons = new();
        }

        public bool HasWeapon(Weapon inWeapon)
        {
            bool hasWeapon = false;

            foreach (Weapon weapon in mountedWeapons)
            {
                if (inWeapon == weapon)
                {
                    hasWeapon = true;
                }             
            }

            return hasWeapon;
        }

        public void FireMount()
        {
            if (mountedWeapons != null || mountedWeapons.Count == 0)
            {
                Debug.Log("Mount was empty");
            }

            foreach (Weapon weapon in mountedWeapons)
            {
                weapon.OnSelectTargets();
            }
        }

        public string GetMountTypeName()
        {
            switch (weaponMountType)
            {
                case WeaponMountType.None:
                    return "None";
                case WeaponMountType.Aux:
                    return "Aux";
                case WeaponMountType.Main:
                    return "Main";
                case WeaponMountType.Flex:
                    return "Flex";
                case WeaponMountType.Heavy:
                    return "Heavy";
                case WeaponMountType.SuperHeavy:
                    return "SuperHeavy";
            }
            return "invalid Mount Type";
        }
    }


    int structure;
    int currentStructure;
    int stress;
    int currentStress;

    int sensors;
    int techAttack;

    int repairCapacity;
    int currentRepairs;

    int saveTarget;

    int speed;
    BaseBehaviour behaviourType;
    Frame mechType;
    int systemPoints;

    int timesOvercharged = 0;

    public Weapon[] weaponsOnThisMech;
    public bool mayFireOrdnance;

    [SerializeField] List<WeaponMount> weaponMounts = new();
    public List<WeaponMount> WeaponMounts => weaponMounts;

    public int remainingActionPoints;
    public int remainingMovement;
    public Team team;

    public bool willBlowUpAtTheEndOfNextTurn;
    public bool blowUpAtTheEndOfThisTurn;
    private Material mechMaterial;

    private List<string> invadeOptions = new List<string> {"Fragment Signal"};


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        team = transform.parent.GetComponent<Team>();
        this.mechType = GetComponent<Frame>();
        this.behaviourType = GetComponent<BaseBehaviour>();

        if ( testPosition != Vector2.zero)
        {
            transform.position = testPosition;
        }
        InitiateMech();
    }

    private void InitiateMech()
    {
        if (mechType == null)
        {
            Debug.Log(name + " saknar frame");
            StartCoroutine(WaitToGetPrimaryTile());
            return;
        }

        AssignMechStats();
        AttachWeaponsToMounts();
        StartCoroutine(WaitToGetPrimaryTile());
    }
    private void AssignMechStats()
    {
        if (mechType == null)
        {
            Debug.Log(name + "saknar frame");
            return;
        }

        objectSize = mechType.objectSize;
        objectName = mechType.objectName;

        structure = mechType.structure;
        currentStructure = structure;
        hp = mechType.hp;
        currentHp = hp;
        stress = mechType.stress;
        currentStress = stress;
        heatCapacity = mechType.heatCapacity;

        armor = mechType.armor;
        evasion = mechType.evasion;
        eDefence = mechType.eDefence;

        sensors = mechType.sensors;
        techAttack = mechType.techAttack;

        repairCapacity = mechType.repairCapacity;
        currentRepairs = repairCapacity;
        saveTarget = mechType.saveTarget;
        speed = mechType.speed;
        systemPoints = mechType.systemPoints;
        weaponMounts = mechType.weaponMounts;
        mechMaterial = GetComponentInChildren<MeshRenderer>().material;
    }
    private void AttachWeaponsToMounts()
    {
        weaponsOnThisMech = GetComponents<Weapon>();

        int i = 0;
        if (weaponMounts == null || weaponsOnThisMech == null)
        {
            Debug.Log("Something was null for AttachWeaponsToMounts() on " + name);
            return;
        }

        foreach (WeaponMount mount in weaponMounts)
        {
            if (i < weaponsOnThisMech.Length)
            {
                mount.AddWeaponToMount(weaponsOnThisMech[i]);
            }
            i++;
        }
    }

    public void attachToTeam(Team inTeam)
    {
        team = inTeam;
        StartCoroutine(WaitToGetPrimaryTile());
    }

    IEnumerator OverheatGlow()
    {
        Color color = mechMaterial.color;
        Color baseColor = color;
        float overheatTimer = 0;

        while (true)
        {
            color.g = Mathf.Lerp(baseColor.g, 0, (Mathf.Cos(overheatTimer) + 1) / 2);
            color.b = Mathf.Lerp(baseColor.b, 0, (Mathf.Cos(overheatTimer) + 1) / 2);
            color.r = Mathf.Lerp(baseColor.r, 1, (Mathf.Cos(overheatTimer) + 1) / 2);
            mechMaterial.color = color;
            overheatTimer += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();

            if (overheatTimer >= MathF.PI * 10)
            {
                overheatTimer = 0;
            }
        }
    }

    // Update is called once per frame
    public override void OnUpdate()
    {
        base.OnUpdate();
        //Debug.Log("Mech OnUpdate got called");
        // create and push the player action menu on the stack!


        if (remainingActionPoints > 0)
        {
            behaviourType.OnUpdate();

            /*
            if (team.m_bIsPlayerTeam)
            {
                PlayerBehaviour();
            }
            else
            {
                EnemyBehaviour();
            }*/
        }
    }

    public override void On0HP()
    {
        currentStructure--;

        if (currentStructure <= 0)
        {
            DestroyBattlefieldObject();
            return;
        }
        MakeStructureDamageCheck();
    }

    public void MakeStructureDamageCheck()
    {
        currentHp += hp;

        int[] rolls = new int[4 - currentStructure];

        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i] = UnityEngine.Random.Range(1, 6);
        }

        int worstRoll = Mathf.Min(rolls);
        Debug.Log("Roll: " + worstRoll);

        int onesCount = 0;
        foreach (int roll in rolls)
        {
            if (roll == 1)
            {
                onesCount++;
            }
        }

        if (onesCount >= 2)
        {
            Debug.Log("Du dog, lul");
            DestroyBattlefieldObject();
        }

        switch (worstRoll)
        {
            case 6:
            case 5:
                Debug.Log("Du vart just Impaired");
                statuses |= 1 << 3; //3 IMPAIRED set to True
                TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Impaired");
                break;
            case 4:
            case 3:
            case 2:
                Debug.Log("System trauma");
                SystemTrauma();
                break;
            case 1:
                Debug.Log("Direct Hit, lul");
                DirectHit();
                break;
        }
        TakeDamage(0);
    }

    private void SystemTrauma()
    {
        bool hasWeapons = false;
        foreach (WeaponMount mount in weaponMounts)
        {
            if (mount.mountedWeapons.Count > 0)
            {
                hasWeapons = true;
            }
        }

        if (hasWeapons)
        {
            Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
            MountBreakMenu mbm = MountBreakMenu.Create(canvas.transform, this);
            Debug.Log(mbm);
            ActionStack.Main.PushAction(mbm);
        }
        else
        {
            Debug.Log("Direct Hit, lul");
            DirectHit();
        }
    }

    private void DirectHit()
    {
        switch (currentStress)
        {
            case >= 3:
                Debug.Log("Du vart just Stunned");
                statuses |= 1 << 10; //10 STUNNED  set to True
                TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Stunned");
                break;

            case 2:
                if (SkillCheck(SkillType.Engineering, 10))
                {
                    Debug.Log("Du vart just Stunned");
                    statuses |= 1 << 10; //10 STUNNED  set to True
                    TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Stunned");
                }
                else
                {
                    DestroyBattlefieldObject();
                }
                break;

            case <= 1:
                DestroyBattlefieldObject();
                break;
        }
    }

    public override void TakeHeat(int inHeat)
    {
        if (heatCapacity <= 0)
        {
            TakeDamage(inHeat);
            return;
        }
        else
        {
            currentHeat += inHeat;
            if (currentHeat >= heatCapacity / 2f)
            {
                statuses |= 1 << 0; //0 DANGERZONE set to True
                Debug.Log("DANGERZONE status check: " + (statuses & 1));

                TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Dangerzone");
            }

            DisplayHeatTaken(inHeat);
            DisplayCurrentHeat();
        }


        if (currentHeat > heatCapacity)
        {

            statuses &= ~(1 << 0); //0 DANGERZONE set to False
            Debug.Log("DANGERZONE status check: " + (statuses & 1));
            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "-Dangerzone");

            currentHeat -= heatCapacity + 1;
            currentStress--;

            MakeOverheatingCheck();
        }
    }

    public void MakeOverheatingCheck()
    {
        int[] rolls = new int[4 - currentStress];

        for (int i = 0; i < rolls.Length; i++)
        {
            rolls[i] = UnityEngine.Random.Range(1, 6);
        }

        int worstRoll = Mathf.Min(rolls);
        Debug.Log("Roll: " + worstRoll);

        int onesCount = 0;
        foreach (int roll in rolls)
        {
            if (roll == 1)
            {
                onesCount++;
            }
        }

        if (onesCount >= 2)
        {
            Debug.Log("Oåterkallelig Härdsmälta, lul");

        }

        switch (worstRoll)
        {
            case 6:
            case 5:
                Debug.Log("Du vart just Jammed");
                statuses |= 1 << 5; //5 JAMMED set to True

                TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Jammed");
                break;
            case 4:
            case 3:
            case 2:
                Debug.Log("Du vart just Exposed");
                statuses |= 1 << 1; //1 EXPOSED set to True
                TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Exposed");
                break;
            case 1:
                Debug.Log("Härdsmälta, lul");

                switch (currentStress)
                {
                    case >= 3:
                        Debug.Log("Du vart just Exposed");
                        statuses |= 1 << 1; //1 EXPOSED set to True
                        TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Exposed");
                        break;
                    case 2:
                        if (SkillCheck(SkillType.Engineering, 10))
                        {
                            Debug.Log("Du vart just Exposed");
                            statuses |= 1 << 1; //1 EXPOSED set to True
                            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "+Exposed");
                        }
                        else
                        {
                            BeginMeltdown();
                        }
                        break;
                    case <= 1:
                        BeginMeltdown();
                        break;
                }
                break;
        }

        Debug.Log("Current Stress: " + currentStress);
        TakeHeat(0);
    }

    public void BeginMeltdown()
    {
        willBlowUpAtTheEndOfNextTurn = true;
        StartCoroutine(OverheatGlow());
    }

    public void Meltdown()
    {
        foreach (HexTile tile in HexCoordGrid.GetInstance().GetHexTilesInRange(2, currentPrimaryTile))
        {
            int damage = UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6); // 4d6
            if (tile.occupied)
            {
                tile.occupiedBy.TakeDamage(damage);
            }
        }

        DestroyBattlefieldObject();
    }

    public override void TakeKnockback(int inKnockbackDistance, HexTile tileToGetPushedFrom)
    {
        //dedede
    }

    public void Stabilize(int stabilizationType)
    {
        switch (stabilizationType)
        {
            case 1: //Clear Heat
                currentHeat = 0;
                statuses &= ~(1 << 2); //2 EXPOSED set to False;
                TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -3f, 0), "-Exposed");
                break;
            case 2: //Repair
                currentHp = hp;
                currentRepairs--;
                break;
            case 3: //Reload
                foreach (WeaponMount mount in weaponMounts)
                {
                    foreach (Weapon weapon in mount.mountedWeapons)
                    {
                        weapon.canShootAgainWithoutLoading = true;
                    }
                }
                break;
            case 4: //Clear Burn
                Debug.Log("Burn är inte verkligt än, vad håller du på med?");
                break;
        }
    }

    public void OnNewTurn()
    {
        remainingActionPoints = 2;
        remainingMovement = speed;
        mayFireOrdnance = true;
        foreach (WeaponMount wm in weaponMounts)
        {
            foreach (Weapon weapon in wm.mountedWeapons)
            {
                weapon.hasfired = false;
            }
        }
        if (willBlowUpAtTheEndOfNextTurn)
        {
            blowUpAtTheEndOfThisTurn = true;
        }
    }

    public void Overcharge()
    {
        remainingActionPoints++;
        timesOvercharged++;

        switch (timesOvercharged)
        {
            case 1:
                TakeHeat(1);
                return;
            case 2:
                TakeHeat(UnityEngine.Random.Range(1, 3));
                return;
            case 3:
                TakeHeat(UnityEngine.Random.Range(1, 6));
                return;
        }
        TakeHeat(UnityEngine.Random.Range(1, 6) + 4);
    }

    public HexTile GetClosestOccupiedTile(HexTile inTile)
    {
        HexTile closestTile = currentPrimaryTile;

        int distance = HexCoordGrid.GetInstance().DistanceBetweenTiles(currentPrimaryTile, inTile);

        foreach (HexTile tile in occupiedTiles)
        {
            int distanceToTile = HexCoordGrid.GetInstance().DistanceBetweenTiles(tile, inTile);
            if ( distanceToTile < distance)
            {
                distance = distanceToTile;
                closestTile = tile;
            }
        }
        return closestTile;
    }

    public int GetSpeed()
    {
        return speed;
    }
    public int GetHealth()
    {
        return hp;
    }
    public int GetCurrentHealth()
    {
        return currentHp;
    }
    public int GetHeatCap()
    {
        return heatCapacity;
    }
    public int GetCurrentHeat()
    {
        return currentHeat;
    }
    public int GetRepairCapacity()
    {
        return repairCapacity;
    }
    public int GetCurrentRepairs()
    {
        return currentRepairs;
    }
    public int GetSensors()
    {
        return sensors;
    }
    public List<string> GetInvadeOptions()
    {
        return invadeOptions;
    }


    public override bool IsDone()
    {
        return remainingActionPoints <= 0;
    }

    public Weapon AddWeapon(Weapon inWeapon)
    {
        if (GetComponent(inWeapon.GetType()))
        {
            return GetComponent(inWeapon.GetType()) as Weapon;
        }

        gameObject.AddComponent(inWeapon.GetType());
        weaponsOnThisMech = GetComponents<Weapon>();
        return gameObject.GetComponent(inWeapon.GetType()) as Weapon;
    }

    public void EmptyMount(WeaponMount inWeaponMount)
    {
        List<Weapon> weaponsThatWereOnTheMount = inWeaponMount.mountedWeapons;
        inWeaponMount.EmptyMount();

        bool otherMountsHaveTheWeapon = false;
        foreach (Weapon weapon in weaponsThatWereOnTheMount)
        {
            foreach (WeaponMount mount in weaponMounts)
            {
                if (mount.HasWeapon(weapon))
                {
                    otherMountsHaveTheWeapon = true;
                }
            }

            if (!otherMountsHaveTheWeapon)
            {
                RemoveWeapon(weapon);
            }
        }
    }

    public void RemoveWeapon(Weapon inWeapon)
    {
        Destroy(inWeapon);
    }
}