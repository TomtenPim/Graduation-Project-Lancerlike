using Actions;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerActionMenu : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;

    [SerializeField] TextMeshProUGUI currentMechName;
    [SerializeField] TextMeshProUGUI healthDisplayText;
    [SerializeField] TextMeshProUGUI energyDisplayText;
    [SerializeField] TextMeshProUGUI heatDisplayText;
    [SerializeField] TextMeshProUGUI movementDisplayText;

    [SerializeField] Button miscButton;
    [SerializeField] Button moveButton;
    [SerializeField] Button getUpButton;
    [SerializeField] Button boostButton;
    [SerializeField] Button attackButton;
    [SerializeField] Button overchargeButton;

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
        currentMechName.text = currentMech.name;

        if (bFirstTime)
        {
            moveButton.interactable = !((1 << 2 & currentMech.statuses) != 0 || (1 << 10 & currentMech.statuses) != 0); // uninteractable if 2 IMMOBILIZED or 10 STUNNED is true.
            attackButton.interactable = !((1 << 5 & currentMech.statuses) != 0 || (1 << 10 & currentMech.statuses) != 0); // uninteractable if 5 JAMMED or 10 STUNNED is true.
            boostButton.interactable = !((1 << 7 & currentMech.statuses) != 0 || (1 << 9 & currentMech.statuses) != 0 || (1 << 10 & currentMech.statuses) != 0);  // uninteractable if 7 PRONE, 9 SLOWED or 10 STUNNED is true.
            overchargeButton.interactable = !((1 << 10 & currentMech.statuses) != 0); // uninteractable if 10 STUNNED is true.
            miscButton.interactable = !((1 << 10 & currentMech.statuses) != 0); // uninteractable if 10 STUNNED is true.
        }

        getUpButton.gameObject.SetActive((1 << 7 & currentMech.statuses) != 0); // set active if 7 PRONE is true.
    }

    public void Update()
    {
        healthDisplayText.text = currentMech.GetCurrentHealth() + "/" + currentMech.GetHealth() + " HP";
        energyDisplayText.text = currentMech.remainingActionPoints + "/2 Energy";
        heatDisplayText.text = currentMech.GetCurrentHeat() + "/" + currentMech.GetHeatCap() + " Heat";
        movementDisplayText.text = currentMech.remainingMovement + "/" + currentMech.GetSpeed();
    }

    #region Properties

    #endregion

    public void OnAttack()
    {
        Debug.Log("OnAttack was Called");
        this.GetComponent<Canvas>().enabled = false;

        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerAttackMenu pam = PlayerAttackMenu.Create(canvas.transform, currentMech);

        ActionStack.Main.PushAction(pam);
    }

    public void OnMisc()
    {
        Debug.Log("OnMisc was Called");
        this.GetComponent<Canvas>().enabled = false;

        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerMiscActionMenu pmam = PlayerMiscActionMenu.Create(canvas.transform, currentMech);

        ActionStack.Main.PushAction(pmam);
    }

    public void OnMove()
    {
        Debug.Log("OnMove was Called");
        this.GetComponent<Canvas>().enabled = false;

        Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
        PlayerMoveMenu pmm = PlayerMoveMenu.Create(canvas.transform, currentMech);

        ActionStack.Main.PushAction(pmm);
    }
    public void OnBoost(Button inButton)
    {
        if (currentMech.remainingActionPoints > 0)
        {
            currentMech.remainingActionPoints--;
            currentMech.remainingMovement += currentMech.GetSpeed();
        }
    }

    public void OnGetUp()
    {
        currentMech.remainingMovement = 0;
        currentMech.statuses &= ~(1 << 7); // remove 7 PRONE
        getUpButton.gameObject.SetActive(false);
    }

    public void OnOvercharge()
    {
        Debug.Log("OnOvercharge was Called");
        currentMech.Overcharge();
    }

    public override bool IsDone()
    {
        return m_bIsDone;
    }

    public void OnGoBack()
    {
        currentMech.remainingActionPoints = 0;
        if (currentMech.blowUpAtTheEndOfThisTurn)
        {
            currentMech.Meltdown();
        }
        currentMech.ClearEndOfTurnStatuses();

        m_bIsDone = true;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerActionMenu Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerActionMenu");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerActionMenu>().currentMech = activeMech;
        return go.GetComponent<PlayerActionMenu>();
    }
}
