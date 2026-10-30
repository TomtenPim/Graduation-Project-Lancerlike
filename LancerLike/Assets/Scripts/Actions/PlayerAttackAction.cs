
using Actions;
using Game;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerAttackAction : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;
    private Button buttonUsedToCallThisAttack = null;

    //this is an ugly fast solution to get the effect of what I want without designing the full weapons system
    private bool isAHMeleeAttack;
    private List<HexTile> tilesInRange;
    private List<HexTile> tilesWithEnemies;

    [SerializeField] TextMeshProUGUI currentMechName;
    [SerializeField] private Material rangeTileMaterial;
    [SerializeField] private Material validTargetTileMaterial;
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
        //this is also an ugly fast solution to get the effect of what I want without designing the full weapons system
        if (isAHMeleeAttack)
        {
            tilesInRange = HexCoordGrid.GetInstance().GetHexTilesInRange(1, currentMech.CurrentPrimaryTile, true);
            HexCoordGrid.GetInstance().DrawTileList(tilesInRange, rangeTileMaterial);
        }
        else
        {
            tilesInRange = HexCoordGrid.GetInstance().GetHexTilesInRange(10, currentMech.CurrentPrimaryTile, true);
            HexCoordGrid.GetInstance().DrawTileList(tilesInRange, rangeTileMaterial);
        }

        foreach (HexTile tile in tilesInRange)
        {
            if (tile.occupiedBy != null)
            {
                HexCoordGrid.GetInstance().DrawTile(tile, validTargetTileMaterial);
            }
        }
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
                HexTile tileAtemptedToTargert = HexCoordGrid.GetInstance().GetHexTileAt(myMouseRay.GetPoint(fPointOfHit));
                foreach (HexTile tile in tilesInRange)
                {
                    if (tile == tileAtemptedToTargert)
                    {
                        currentMech.remainingActionPoints--;
                        RollToHit(tile.occupiedBy);
                        m_bIsDone = true;
                    }
                }
            }
           
        }
    }

    public void OnGoBack()
    {
        if (buttonUsedToCallThisAttack != null)
        {
            buttonUsedToCallThisAttack.interactable = true;
        }
        m_bIsDone = true;
    }

    private void RollToHit(BattlefieldObject target)
    {
        if (target == null)
        {
            Debug.Log("You missed Completely");
            return;
        }

        if (UnityEngine.Random.Range(1, 20) >= target.Evasion)
        {
            if (isAHMeleeAttack)
            {
                target.TakeDamage(UnityEngine.Random.Range(1, 6) + UnityEngine.Random.Range(1, 6) + 1);
            }
            else
            {
                target.TakeDamage(Mathf.Max(UnityEngine.Random.Range(1, 6), 2));
            }
        }
        else
        {
            if (isAHMeleeAttack)
            {
                Debug.Log("You missed");

            }
            else
            {
                Debug.Log("realiablity is usefull");
                target.TakeDamage(2);
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        HexCoordGrid.GetInstance().ResetTileList(tilesInRange);
        // det känns som att det borde finnas något effektivare sätt att göra detta på. Den där söksträngen är väldigt lång.
        transform.parent.gameObject.GetComponentInChildren<PlayerAttackMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerAttackAction Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerAttackAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerAttackAction>().currentMech = activeMech;
        return go.GetComponent<PlayerAttackAction>();
    }

    public static PlayerAttackAction Create(Transform parent, Mech activeMech, bool isHMelee, Button callerButton)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerAttackAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        PlayerAttackAction newAttackAction = go.GetComponent<PlayerAttackAction>();

        newAttackAction.currentMech = activeMech;
        newAttackAction.isAHMeleeAttack = isHMelee;
        newAttackAction.buttonUsedToCallThisAttack = callerButton;
        return go.GetComponent<PlayerAttackAction>();
    }
}
