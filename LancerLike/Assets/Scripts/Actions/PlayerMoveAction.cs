
using Actions;
using Game;
using Math;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class PlayerMoveMenu : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    private List<HexTile> validTiles;
    [SerializeField] TextMeshProUGUI currentMechName;
    [SerializeField] private Material walkableTileMaterial;

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
        //validTiles = HexCoordGrid.GetInstance().GetHexTilesInRange(currentMech.remainingMovement, currentMech.CurrentPrimaryTile);
        validTiles = HexCoordGrid.GetInstance().BreadthSearchTilesInRange(currentMech.CurrentPrimaryTile, currentMech, currentMech.remainingMovement);

        InvalidateTilesYouCantGoTo();

        HexCoordGrid.GetInstance().DrawTileList(validTiles, walkableTileMaterial);
    }

    private void InvalidateTilesYouCantGoTo()
    {
        List<HexTile> tempTiles = new(validTiles);
        foreach (HexTile tile in tempTiles)
        {
            /*if (tile.occupied == true && tile.occupiedBy != currentMech)
            {
                foreach (HexTile invalidTile in GetTilesToInvalidate(tile, currentMech.ObjectSize))
                {
                    validTiles.RemoveAll(x => x == invalidTile);
                }
            }*/

            if (!HexCoordGrid.GetInstance().CanAMechStayHere(tile, currentMech))
            {
                validTiles.Remove(tile);
            }
        }
    }

    private void InvalidateTiles(HexTile invalidBaseTile)
    {
        List<HexTile> TilesToInvalidate = new();

        foreach (HexTile tile in GetTilesToInvalidate(invalidBaseTile, currentMech.ObjectSize))
        {
            validTiles.RemoveAll(x => x == tile);
        }
    }

    private List<HexTile> GetTilesToInvalidate(HexTile inInvalidBaseTile, BattlefieldObject.Size inMechSize)
    {
        List<HexTile> TilesToInvalidate = new();

        switch (inMechSize)
        {
            case BattlefieldObject.Size.Two:
                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord.GetNeighbor(HexCoord.Directions.SouthWest)));
                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord.GetNeighbor(HexCoord.Directions.SouthEast)));
                TilesToInvalidate.Add(inInvalidBaseTile);
                break;

            case BattlefieldObject.Size.Three:
                foreach (HexCoord hexCoord in inInvalidBaseTile.hexCoord.Neighbors)
                {
                    TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(hexCoord));
                }
                break;

            case BattlefieldObject.Size.Four:
                foreach (HexCoord hexCoord in inInvalidBaseTile.hexCoord.Neighbors)
                {
                    TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(hexCoord));
                }
                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord + new HexCoord(-1, -1, 2)));

                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord + new HexCoord(-2, 0, 2)));
                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord + new HexCoord(1, -2, 1)));
                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord + new HexCoord(2, 0, -2)));

                TilesToInvalidate.Add(HexCoordGrid.GetInstance().GetHexTileAt(inInvalidBaseTile.hexCoord + new HexCoord(2, -1, -1)));
                break;

            case BattlefieldObject.Size.Five:
                foreach (HexTile tile in HexCoordGrid.GetInstance().GetHexTilesInRange(2, inInvalidBaseTile))
                {
                    TilesToInvalidate.Add(tile);
                }
                break;
        }

        if (TilesToInvalidate.Count < 2)
        {
            TilesToInvalidate.Add(inInvalidBaseTile);
        }

        return TilesToInvalidate;
    }


    public override void OnUpdate()
    {
        base.OnUpdate();

        if (Input.GetMouseButtonDown(0))
        {
            Plane ground = new Plane(Vector3.up, new Vector3(0, -0.5f, 0));
            float fPointOfHit;

            Ray myMouseRay = Battlefield.sm_instance.MouseRay;
            if (ground.Raycast(myMouseRay, out fPointOfHit))
            {
                HexTile tileAtemptedToGoTo = HexCoordGrid.GetInstance().GetHexTileAt(myMouseRay.GetPoint(fPointOfHit));
                foreach (HexTile tile in validTiles)
                {
                    if (tile == tileAtemptedToGoTo)
                    {
                        currentMech.remainingMovement -= HexCoordGrid.GetInstance().AStarPathingToTile(currentMech.CurrentPrimaryTile, currentMech, tileAtemptedToGoTo, false).Count;
                        currentMech.SetPrimaryTile(tileAtemptedToGoTo);
                        currentMech.transform.position = currentMech.CurrentPrimaryTile.GetWorldPosition();
                        Battlefield.sm_instance.m_camera.ChangeFocus(currentMech.gameObject);
                        m_bIsDone = true;
                        currentMech.mayFireOrdnance = false;
                    }
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
        HexCoordGrid.GetInstance().ResetTileList(validTiles);
        // det känns som att det borde finnas något effektivare sätt att göra detta på. Den där söksträngen är väldigt lång.
        transform.parent.gameObject.GetComponentInChildren<PlayerActionMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerMoveMenu Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerMoveMenu");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerMoveMenu>().currentMech = activeMech;
        return go.GetComponent<PlayerMoveMenu>();
    }
}
