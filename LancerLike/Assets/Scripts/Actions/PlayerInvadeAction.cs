
using Actions;
using Game;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;


public class PlayerInvadeAction : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;

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

        tilesInRange = HexCoordGrid.GetInstance().GetHexTilesInRange(currentMech.GetSensors(), currentMech.OccupiedTiles, true);

        foreach (HexTile tile in tilesInRange)
        {
            HexCoordGrid.GetInstance().DrawTile(tile, rangeTileMaterial);
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
                //HexCoordGrid.GetInstance().DrawTile(hexTileCurrentlyPointingAt, validTargetTileMaterial);
            }

            if (hexTileCurrentlyPointingAt != mouseHex && mouseHex != null)
            {
                if (HexCoordGrid.GetInstance().DistanceBetweenTiles(currentMech.CurrentPrimaryTile, mouseHex) <= currentMech.GetSensors())
                {
                    HexCoordGrid.GetInstance().DrawTile(hexTileCurrentlyPointingAt, rangeTileMaterial);
                    hexTileCurrentlyPointingAt = mouseHex;
                    HexCoordGrid.GetInstance().DrawTile(hexTileCurrentlyPointingAt, validTargetTileMaterial);
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
                bool madeAnInvadeAgainstATarget = false;

                foreach (HexTile targetedTile in tilesAtemptedToTargert)
                {
                    if (targetedTile.occupiedBy?.GetType() == typeof(Mech))
                    {
                        Debug.Log("Invaded: " + targetedTile.occupiedBy);

                        TechAttachRoll(currentMech, (Mech)targetedTile.occupiedBy, 0,0);

                        madeAnInvadeAgainstATarget = true;
                    }
                }

                Debug.Log(tileAtemptedToTargert);

                if (madeAnInvadeAgainstATarget)
                {
                    currentMech.remainingActionPoints--;
                    currentMech.mayFireOrdnance = false;
                    m_bIsDone = true;
                }
            }
        }
    }

    private void TechAttachRoll(Mech currentMech , Mech target, int inAccuracy, int inDifficulty)
    {
        if (target.EDefence == -1)
        {
            return;
        }

        float totalBonusDice = inAccuracy + ((target.statuses) & 1 << 7) >> 7; // +1 accuracy if target has 7 PRONE
        totalBonusDice -= inDifficulty + ((currentMech.statuses) & 1 << 3) >> 3; // +1 difficulty if attacker has 3 IMPAIRED

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

        if (UnityEngine.Random.Range(1, 20) + largestBonusRoll >= target.EDefence)
        {
            Invade(currentMech, target);
        }
        else
        {
            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, target.transform.position, "Miss");
        }
    }

    private void Invade(Mech currentMech, Mech target)
    {
        if (currentMech.GetInvadeOptions().Contains("Fragment Signal"))
        {
            if (currentMech.team != target.team)
            {
                target.TakeHeat(2);
            }
            target.statuses |= 1 << 3; //3 IMPAIRED set to True
            target.statuses |= 1 << 9; //9 SLOWED set to True
            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, target.transform.position + new Vector3(0, -2f, 0), "+Impaired");
            TextPopup.Create(Battlefield.sm_instance.GetComponentInChildren<Canvas>().transform, target.transform.position + new Vector3(0, -4f, 0), "+Slowed");

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
        transform.parent.gameObject.GetComponentInChildren<PlayerAttackMenu>().GetComponent<Canvas>().enabled = true;

        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerInvadeAction Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerInvadeAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        PlayerInvadeAction newPlayerInvadeAction = go.GetComponent<PlayerInvadeAction>();
        newPlayerInvadeAction.currentMech = activeMech;
        return newPlayerInvadeAction;
    }
}
