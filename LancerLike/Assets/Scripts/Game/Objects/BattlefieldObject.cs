using Actions;
using Game;
using Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static UnityEngine.GraphicsBuffer;

public class BattlefieldObject : ActionStack.ActionBehavior
{
    public enum Size
    {
        Half,
        One,
        Two,
        Three,
        Four,
        Five
    }

    [Flags]
    public enum Status
    {
        DANGERZONE = 1, //0
        EXPOSED = 2,    //1
        IMMOBILIZED = 4,//2
        IMPAIRED = 8,   //3
        INVISIBLE = 16, //4
        JAMMED = 32,    //5
        LOCKON = 64,    //6
        PRONE = 128,    //7
        SHREDDED = 256, //8
        SLOWED = 512,   //9
        STUNNED = 1024, //10
    }

    public enum SkillType
    {
        none,
        Hull,
        Agility,
        Systems,
        Engineering
    }

    [SerializeField] protected Size objectSize;
    [SerializeField] protected string objectName;
    [SerializeField] protected int hp;
    protected int currentHp;
    [SerializeField] protected int heatCapacity;
    protected int currentHeat = 0;
    [SerializeField] protected int armor = 0;
    [SerializeField] protected int evasion = 8;
    [SerializeField] protected int eDefence = 8;
    protected HexTile currentPrimaryTile = null;
    [SerializeField] protected List<HexTile> occupiedTiles = new();
    [SerializeField] protected Vector2 testPosition;
    public short statuses;
    protected List<SkillType> accurateSkills = new();
    protected List<SkillType> difficultSkills = new();
    public Status StatusesEnum => (Status)statuses;

    public HexTile CurrentPrimaryTile => currentPrimaryTile;
    public List<HexTile> OccupiedTiles => occupiedTiles;
    public int Evasion => evasion;
    public int EDefence => eDefence;
    public int Armor => armor;
    public Size ObjectSize => objectSize;

    public virtual void Start()
    {
        currentHp = hp;
        StartCoroutine(WaitToGetPrimaryTile());
    }
    protected void Update()
    {
    }

    public bool SkillCheck(SkillType skillcheckType, int difficulty)
    {
        float totalBonusDice = 0;
        if (accurateSkills.Contains<SkillType>(skillcheckType))
        {
            totalBonusDice++;
        }
        if (difficultSkills.Contains<SkillType>(skillcheckType))
        {
            totalBonusDice--;
        }
        totalBonusDice -= (statuses & 1 << 3) >> 3; // +1 difficulty if attacker has 3 IMPAIRED

        float largestBonusRoll = 0;
        for (int i = 0; i < Mathf.Abs(totalBonusDice); i++)
        {
            largestBonusRoll = Mathf.Max(largestBonusRoll, UnityEngine.Random.Range(1, 6));
        }
        largestBonusRoll = largestBonusRoll * largestBonusRoll / MathF.Abs(largestBonusRoll);
        return UnityEngine.Random.Range(1, 20) + largestBonusRoll >= difficulty;
    }

    protected IEnumerator WaitToGetPrimaryTile()
    {
        Debug.Log("Scene name: " + this.gameObject.scene.name);
        if (this.gameObject.scene.name == "HexGrids")
        {
            while (currentPrimaryTile == null)
            {
                yield return new WaitForEndOfFrame();

                SetPrimaryTile(HexCoordGrid.GetInstance().GetHexTileAt(transform.position));

                if (currentPrimaryTile != null)
                {
                    this.transform.position = currentPrimaryTile.GetWorldPosition();
                }
            }
        }
    }

    public void SetPrimaryTile(HexTile primaryTile)
    {

        if (currentPrimaryTile != null)
        {
            currentPrimaryTile.UpdateOccupiedStatus(false);

        }
        occupiedTiles.Remove(currentPrimaryTile);
        currentPrimaryTile = primaryTile;
        occupiedTiles.Add(primaryTile);
        currentPrimaryTile.UpdateOccupiedStatus(true, this);
        SetSecondaryTiles();
    }

    private void SetSecondaryTiles()
    {
        if (objectSize < Size.Two)
        {
            return;
        }

        foreach (HexTile tile in occupiedTiles)
        {
            tile.UpdateOccupiedStatus(false);
        }
        occupiedTiles.Clear();

        switch (objectSize)
        {
            case Size.Two:
                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord.GetNeighbor(HexCoord.Directions.NorthWest)));
                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord.GetNeighbor(HexCoord.Directions.NorthEast)));
                occupiedTiles.Add(currentPrimaryTile);
                break;

            case Size.Three:
                foreach (HexCoord hexCoord in currentPrimaryTile.hexCoord.Neighbors)
                {
                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(hexCoord));
                }
                occupiedTiles.Add(currentPrimaryTile);
                break;

            case Size.Four:
                foreach (HexCoord hexCoord in currentPrimaryTile.hexCoord.Neighbors)
                {
                    occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(hexCoord));
                }
                occupiedTiles.Add(currentPrimaryTile);

                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord + new HexCoord(1, 1, -2)));
                
                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord + new HexCoord(2, 0, -2)));
                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord + new HexCoord(-1, 2, -1)));
                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord + new HexCoord(-2, 0, 2)));
                
                occupiedTiles.Add(HexCoordGrid.GetInstance().GetHexTileAt(currentPrimaryTile.hexCoord + new HexCoord(-2, 1, 1)));
                break;

            case Size.Five:
                foreach (HexTile tile in HexCoordGrid.GetInstance().GetHexTilesInRange(2, currentPrimaryTile))
                {
                    occupiedTiles.Add(tile);
                }
                break;

        }



        UpdateOccupiedStatusOfOccupiedTiles();
    }

    private void UpdateOccupiedStatusOfOccupiedTiles()
    {
        foreach (HexTile tile in occupiedTiles)
        {
            tile.UpdateOccupiedStatus(true, this);
        }
    }



    public void TakeDamage(int inDamage)
    {
        if(hp == -1)
        {
            return;
        } 

        int damageToTake = inDamage;
        if ((1 << 1 & statuses) != 0) // if 1 EXPOSED is true.
        {
            damageToTake *= 2;
        }

        currentHp -= damageToTake - armor * (((~statuses) & 1 << 8) >> 8); // multiplies armour by 0 if 8 SHREDDED is true;
        DisplayDamageTaken(damageToTake - armor * (((~statuses) & 1 << 8) >> 8)); // multiplies armour by 0 if 8 SHREDDED is true;
        DisplayCurrentHealth();

        Debug.Log(name + " has " + currentHp + " hp left .");

        if (currentHp <= 0)
        {
            On0HP();
        }
    }

    public virtual void On0HP()
    {
        DestroyBattlefieldObject();
    }

    public virtual void DestroyBattlefieldObject()
    {
        foreach(HexTile tile in occupiedTiles)
        {
            tile?.UpdateOccupiedStatus(false);
        }
        Debug.Log(name + " died.");
        this.gameObject.SetActive(false);
    }


    public virtual void TakeHeat(int inHeat)
    {
        TakeDamage(inHeat);
    }

    public virtual void TakeKnockback(int inKnockbackDistance, HexTile tileToGetPushedFrom)
    {

    }

    public void ClearEndOfTurnStatuses()
    {
        if ((1 << 3 & statuses) != 0)
        {
            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, 1f, 0), "-Impaired");
            statuses &= ~(1 << 3); //3 IMPAIRED set to False;
        }
        if ((1 << 10 & statuses) != 0)
        {
            statuses &= ~(1 << 10); //10 STUNNED set to False;
            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, this.transform.position + new Vector3(0, -1f, 0), "-Stunned");
        }
    }

    public void DisplayDamageTaken(int inDamageTaken)
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        string constructedString = "-" + inDamageTaken;

        TextPopup.Create(canvas.transform, this.transform.position + new Vector3(0, 1f, 0), constructedString);
    }

    public void DisplayCurrentHealth()
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        string constructedString = currentHp + "/" + hp;
        TextPopup.Create(canvas.transform, this.transform.position + new Vector3(0, -1f, 0), constructedString);
    }

    public void DisplayHeatTaken(int inHeatTaken)
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        string constructedString = "+" + inHeatTaken;

        TextPopup.Create(canvas.transform, this.transform.position + new Vector3(0, 1f, 0), constructedString, Color.orange);
    }

    public void DisplayCurrentHeat()
    {
        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        string constructedString = currentHeat + "/" + heatCapacity;
        TextPopup.Create(canvas.transform, this.transform.position + new Vector3(0, -1f, 0), constructedString, Color.orange);
    }


}



