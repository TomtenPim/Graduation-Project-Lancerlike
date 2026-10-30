using Actions;
using Game;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStabilizeAction : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;

    [SerializeField] TextMeshProUGUI currentMechName;
    [SerializeField] TextMeshProUGUI currentRepairsText;
    [SerializeField] TextMeshProUGUI currentStabilizeSelectedOptionsCountText;

    [SerializeField] Button heatButton;
    [SerializeField] Button repairButton;
    [SerializeField] Button reloadButton;
    [SerializeField] Button burnButton;
    [SerializeField] Button confirmButton;

    private enum SelectionForStabilization
    {
        None = 0,

        ClearHeat = 1,
        Repair = 2,

        Reload = 3,
        ClearBurn = 4
    }

    private SelectionForStabilization firstChoice = 0;
    private SelectionForStabilization secondChoice = 0;

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
        currentMechName.text = currentMech.name;
        currentRepairsText.text = currentMech.GetCurrentRepairs() + "/" + currentMech.GetRepairCapacity();

        repairButton.interactable = (currentMech.GetCurrentRepairs() > 0);
    }

    /*public void Update()
    {
        healthDisplayText.text = currentMech.GetCurrentHealth() + "/" + currentMech.GetHealth() + " HP";
        energyDisplayText.text = currentMech.remainingActionPoints + "/2 Energy";
        heatDisplayText.text = currentMech.GetCurrentHeat() + "/" + currentMech.GetHeatCap() + " Heat";
        movementDisplayText.text = currentMech.remainingMovement + "/" + currentMech.GetSpeed();
    }*/

    #region Properties

    #endregion

    public void OnClearHeat()
    {
        firstChoice = SelectionForStabilization.ClearHeat;
        CheckIfSufficientChoicesHasBeenPicked();
    }

    public void OnRepair()
    {
        firstChoice = SelectionForStabilization.Repair;
        CheckIfSufficientChoicesHasBeenPicked();
    }

    public void OnReload()
    {
        secondChoice = SelectionForStabilization.Reload;
        CheckIfSufficientChoicesHasBeenPicked();
    }

    public void OnClearBurn()
    {
        secondChoice = SelectionForStabilization.ClearBurn;
        CheckIfSufficientChoicesHasBeenPicked();
    }

    public void CheckIfSufficientChoicesHasBeenPicked()
    {
        confirmButton.interactable = (firstChoice != 0 && secondChoice != 0);
        currentStabilizeSelectedOptionsCountText.text =  ( Convert.ToInt16(firstChoice != 0) + Convert.ToInt16(secondChoice != 0)) +"/2";

    }

    public void OnConfirmStabilization()
    {
        currentMech.remainingActionPoints -= 2;
        currentMech.mayFireOrdnance = false;

        currentMech.Stabilize((int)firstChoice);
        currentMech.Stabilize((int)secondChoice);

        m_bIsDone = true;
    }
    
    

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public void OnGoBack()
    {
        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        transform.parent.gameObject.GetComponentInChildren<PlayerMiscActionMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerStabilizeAction Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerStabilizeAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerStabilizeAction>().currentMech = activeMech;
        return go.GetComponent<PlayerStabilizeAction>();
    }
}
