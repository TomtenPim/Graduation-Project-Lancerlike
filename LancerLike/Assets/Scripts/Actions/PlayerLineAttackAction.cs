
using Actions;
using Game;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerLineAttackAction : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    private Weapon currentWeapon;

    private List<HexTile> tilesInRange;

    [SerializeField] TextMeshProUGUI currentMechName;
    [SerializeField] private Material rangeTileMaterial;
    [SerializeField] private Material validTargetTileMaterial;
    #region Properties

    #endregion
    private HexTile hexTileCurrentlyPointingAt;
    private List<HexTile> hexTilesOnTheLine = new();

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
        currentMechName.text = currentMech.name;

        tilesInRange = HexCoordGrid.GetInstance().GetHexTilesInRange(currentWeapon.range, currentMech.OccupiedTiles, true);

        foreach (HexTile tile in tilesInRange)
        {
            HexCoordGrid.GetInstance().DrawTile(tile, rangeTileMaterial);
 
            /*
            if (tile.occupiedBy != null)
            {
                {
                    HexCoordGrid.GetInstance().DrawTile(tile, validTargetTileMaterial);
                }
            }*/
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

            if (hexTileCurrentlyPointingAt == null)
            {
                hexTileCurrentlyPointingAt = mouseHex;
            }

            if (hexTileCurrentlyPointingAt != mouseHex && mouseHex != null)
            {
                if (currentMech.OccupiedTiles.Contains(mouseHex))
                {
                    return;
                }

                HexTile closestOccupiedTile = currentMech.GetClosestOccupiedTile(hexTileCurrentlyPointingAt);

                Vector3 vectorBetweenTiles = mouseHex.GetWorldPosition() - closestOccupiedTile.GetWorldPosition();
                vectorBetweenTiles = vectorBetweenTiles / HexCoordGrid.GetInstance().DistanceBetweenTiles(closestOccupiedTile, mouseHex);
                HexTile furthestHexOnTheLine = null;
                int retries = 0;
                while (furthestHexOnTheLine == null)
                {
                    furthestHexOnTheLine = HexCoordGrid.GetInstance().GetHexTileAt(closestOccupiedTile.GetWorldPosition() + vectorBetweenTiles * (currentWeapon.range - retries));
                    retries++;
                }

                HexCoordGrid.GetInstance().DrawTileList(hexTilesOnTheLine, rangeTileMaterial);
                hexTileCurrentlyPointingAt = mouseHex;
                hexTilesOnTheLine = HexCoordGrid.GetInstance().GetHexTilesBetween(closestOccupiedTile, furthestHexOnTheLine, 1);
                HexCoordGrid.GetInstance().DrawTileList(hexTilesOnTheLine, validTargetTileMaterial);

            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            List<HexTile> tilesAtemptedToTargert = hexTilesOnTheLine;

            if (tilesAtemptedToTargert != null)
            {
                bool madeAnAttackAgainstATarget = false;

                foreach (HexTile targetedTile in tilesAtemptedToTargert)
                {

                    if (targetedTile.occupied)
                    {
                        madeAnAttackAgainstATarget = true;
                        currentWeapon.OnHitRoll(targetedTile.occupiedBy);
                    }

                }


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
        // det känns som att det borde finnas något effektivare sätt att göra detta på. Den där söksträngen är väldigt lång.
        transform.parent.gameObject.GetComponentInChildren<PlayerAttackMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerLineAttackAction Create(Transform parent, Weapon activeWeapon)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerLineAttackAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        PlayerLineAttackAction newPlayerLineAttackAction = go.GetComponent<PlayerLineAttackAction>();
        newPlayerLineAttackAction.currentWeapon = activeWeapon;
        newPlayerLineAttackAction.currentMech = activeWeapon.GetComponent<Mech>();
        return newPlayerLineAttackAction;
    }

    /*public static PlayerLineAttackAction Create(Transform parent, Mech activeMech, Weapon activeWeapon)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerLineAttackAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerLineAttackAction>().currentMech = activeMech;
        go.GetComponent<PlayerLineAttackAction>().currentWeapon = activeWeapon;
        return go.GetComponent<PlayerLineAttackAction>();
    }

    /*public static PlayerLineAttackAction Create(Transform parent, Mech activeMech, bool isHMelee, Button callerButton)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerAttackAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        PlayerLineAttackAction newAttackAction = go.GetComponent<PlayerLineAttackAction>();

        newAttackAction.currentMech = activeMech;
        newAttackAction.isAHMeleeAttack = isHMelee;
        newAttackAction.buttonUsedToCallThisAttack = callerButton;
        return go.GetComponent<PlayerLineAttackAction>();
    }*/
}
