using Actions;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MechCustomizationButton : MonoBehaviour
{

    Mech currentMech;

    public void Start()
    {
        currentMech = GetComponentInChildren<Mech>();
    }


    public void OnPressed()
    {
        Debug.Log("OnPressed was Called");

        Canvas canvas =  transform.parent.GetComponent<Canvas>();
        MechCustmizationAction mca = MechCustmizationAction.Create(canvas.transform, currentMech);

        ActionStack.Main.PushAction(mca);
    }
}
