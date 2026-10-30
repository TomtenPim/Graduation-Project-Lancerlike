
using Actions;
using Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerNormalAttackAction : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    private Weapon currentWeapon;

    private List<HexTile> tilesInRange;

    [SerializeField] TextMeshProUGUI currentMechName;
    [SerializeField] private Material rangeTileMaterial;
    [SerializeField] private Material validTargetTileMaterial;

    private HexTile hexTileCurrentlyPointingAt;
    private List<HexTile> hexTilesWithinBlastRangeOfTileCurrentlyPointingAt = new();

    #region Properties

    #endregion

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
        currentMechName.text = currentMech.name;

        tilesInRange = HexCoordGrid.GetInstance().GetHexTilesInRange(currentWeapon.range, currentMech.OccupiedTiles, true);
        if (!currentWeapon.ignoresLineOfSight)
        {
            tilesInRange = HexCoordGrid.GetInstance().RemoveTilesOutsideOfLineOfSight(tilesInRange, currentMech, true);
        }

        foreach (HexTile tile in tilesInRange)
        {
            HexCoordGrid.GetInstance().DrawTile(tile, rangeTileMaterial);

            if (tile.occupiedBy != null)
            {
                /*
                //&& tile.occupiedBy.GetType() is Mech
                //if (tile.occupiedBy.GetType() is Mech)
                {
                    HexCoordGrid.GetInstance().DrawTile(tile, validTargetTileMaterial);
                }*/

            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        Plane ground = new Plane(Vector3.up, new Vector3(0, -0.5f, 0));
        float fPointOfHit;

        Ray myMouseRay = Battlefield.sm_instance.MouseRay;
        if (ground.Raycast(myMouseRay, out fPointOfHit))
        {
            HexTile mouseHex = HexCoordGrid.GetInstance().GetHexTileAt(myMouseRay.GetPoint(fPointOfHit));

            if (hexTileCurrentlyPointingAt == null && mouseHex != null)
            {
                hexTileCurrentlyPointingAt = mouseHex;
                //HexCoordGrid.GetInstance().DrawTile(hexTileCurrentlyPointingAt, validTargetTileMaterial);
            }

            if (currentWeapon.isBlast)
            {
                if (hexTileCurrentlyPointingAt != mouseHex && mouseHex != null)
                {
                    //if (HexCoordGrid.GetInstance().DistanceBetweenTiles(currentMech.GetClosestOccupiedTile(mouseHex), mouseHex) <= currentWeapon.range)
                    if (tilesInRange.Contains(mouseHex))
                    {
                            foreach (HexTile tile in hexTilesWithinBlastRangeOfTileCurrentlyPointingAt)
                        {
                            if (tilesInRange.Contains(tile))
                            {
                                HexCoordGrid.GetInstance().DrawTile(tile, rangeTileMaterial);
                            }
                            else
                            {
                                HexCoordGrid.GetInstance().ResetTile(tile);
                            }
                        }
                        hexTilesWithinBlastRangeOfTileCurrentlyPointingAt = HexCoordGrid.GetInstance().GetHexTilesInRange(currentWeapon.blastRange, mouseHex);
                        hexTileCurrentlyPointingAt = mouseHex;
                        HexCoordGrid.GetInstance().DrawTileList(hexTilesWithinBlastRangeOfTileCurrentlyPointingAt, validTargetTileMaterial);
                    }
                }
            }
            else
            {
                if (hexTileCurrentlyPointingAt != mouseHex && mouseHex != null)
                {
                    if (HexCoordGrid.GetInstance().DistanceBetweenTiles(currentMech.CurrentPrimaryTile, mouseHex) <= currentWeapon.range)
                    {
                        if (tilesInRange.Contains(hexTileCurrentlyPointingAt))
                        {
                            HexCoordGrid.GetInstance().DrawTile(hexTileCurrentlyPointingAt, rangeTileMaterial);
                        }
                        else
                        {
                            HexCoordGrid.GetInstance().ResetTile(hexTileCurrentlyPointingAt);
                        }
                            hexTileCurrentlyPointingAt = mouseHex;
                        HexCoordGrid.GetInstance().DrawTile(hexTileCurrentlyPointingAt, validTargetTileMaterial);
                    }
                }
            }

        }

        if (Input.GetMouseButtonDown(0))
        {
            HexTile tileAtemptedToTargert = hexTileCurrentlyPointingAt;
            List<HexTile> tilesAtemptedToTargert = new();
            tilesAtemptedToTargert.Add(tileAtemptedToTargert);

            if (tileAtemptedToTargert != null)
            {
                bool madeAnAttackAgainstATarget = false;

                if (currentWeapon.isBlast)
                {
                    tilesAtemptedToTargert.Remove(tileAtemptedToTargert);
                    tilesAtemptedToTargert = HexCoordGrid.GetInstance().GetHexTilesInRange(currentWeapon.blastRange, tileAtemptedToTargert);
                }


                List<BattlefieldObject> struckObjects = new();

                foreach (HexTile tile in tilesAtemptedToTargert)
                {
                    if (tile.occupied)
                    {
                        madeAnAttackAgainstATarget = true;
                        if (!struckObjects.Contains(tile.occupiedBy))
                        {
                            struckObjects.Add(tile.occupiedBy);
                        }
                    }
                }

                /*foreach (HexTile tile in tilesInRange)
                {
                    if (tilesAtemptedToTargert.Contains(tile) && tile.occupied)
                    {
                        madeAnAttackAgainstATarget = true;
                        if (!struckObjects.Contains(tile.occupiedBy))
                        {
                            struckObjects.Add(tile.occupiedBy);
                        }
                    }
                }*/

                foreach (BattlefieldObject target in struckObjects)
                {
                    currentWeapon.OnHitRoll(target);
                }

                Debug.Log(tileAtemptedToTargert);

                if (madeAnAttackAgainstATarget)
                {
                    currentMech.remainingActionPoints--;
                    currentWeapon.OnAttack();
                    m_bIsDone = true;
                }
            }
        }
    }

    public void OnGoBack()
    {
        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        HexCoordGrid.GetInstance().ResetTileList(tilesInRange);
        if (currentWeapon.isBlast)
        {
            HexCoordGrid.GetInstance().ResetTileList(hexTilesWithinBlastRangeOfTileCurrentlyPointingAt);
        }
        transform.parent.gameObject.GetComponentInChildren<PlayerAttackMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerNormalAttackAction Create(Transform parent, Weapon activeWeapon)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerNormalAttackAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        PlayerNormalAttackAction newPlayerNormalAttackAction = go.GetComponent<PlayerNormalAttackAction>();
        newPlayerNormalAttackAction.currentWeapon = activeWeapon;
        newPlayerNormalAttackAction.currentMech = activeWeapon.GetComponent<Mech>();
        return newPlayerNormalAttackAction;
    }
    /* public static PlayerNormalAttackAction Create(Transform parent, Mech activeMech, Weapon activeWeapon)
     {
         GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerNormalAttackAction");
         GameObject go = Instantiate(prefab);
         go.transform.SetParent(parent, false);
         go.GetComponent<PlayerNormalAttackAction>().currentMech = activeMech;
         go.GetComponent<PlayerNormalAttackAction>().currentWeapon = activeWeapon;
         return go.GetComponent<PlayerNormalAttackAction>();
     }

     /*public static PlayerNormalAttackAction Create(Transform parent, Mech activeMech, bool isHMelee, Button callerButton)
     {
         GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerNormalAttackAction");
         GameObject go = Instantiate(prefab);
         go.transform.SetParent(parent, false);
         PlayerNormalAttackAction newAttackAction = go.GetComponent<PlayerNormalAttackAction>();

         newAttackAction.currentMech = activeMech;
         newAttackAction.isAHMeleeAttack = isHMelee;
         newAttackAction.buttonUsedToCallThisAttack = callerButton;
         return go.GetComponent<PlayerNormalAttackAction>();
     }*/
}
