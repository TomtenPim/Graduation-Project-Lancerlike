using Actions;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMiscActionMenu : ActionStack.ActionBehavior
{
    private bool m_bIsDone;
    private Mech currentMech;

    [SerializeField] TextMeshProUGUI currentMechName;

    [SerializeField] Button StabiliseButton;

    public override void OnBegin(bool bFirstTime)
    {
        base.OnBegin(bFirstTime);
        currentMechName.text = currentMech.name;

        StabiliseButton.interactable = (currentMech.remainingActionPoints > 1);
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

    public void OnStabilize(Button inButton)
    {
        if (currentMech.remainingActionPoints > 1)
        {
            this.GetComponent<Canvas>().enabled = false;

            Canvas canvas = Battlefield.sm_instance.GetComponentInChildren<Canvas>();
            PlayerStabilizeAction psa = PlayerStabilizeAction.Create(canvas.transform, currentMech);

            ActionStack.Main.PushAction(psa);
        }
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
        transform.parent.gameObject.GetComponentInChildren<PlayerActionMenu>().GetComponent<Canvas>().enabled = true;
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static PlayerMiscActionMenu Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/PlayerMiscMenu");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.GetComponent<PlayerMiscActionMenu>().currentMech = activeMech;
        return go.GetComponent<PlayerMiscActionMenu>();
    }
}
