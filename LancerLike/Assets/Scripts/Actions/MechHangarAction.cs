using Actions;
using NUnit.Framework;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechHangarAction : ActionStack.ActionBehavior
{
    [SerializeField] private GameObject safeObject;

    private void Start()
    {
        ActionStack.Main.PushAction(this);
    }

    public void OnGo()
    {
        //ActionStack.Main.RemoveAction(this);


        SaveMechs();

        

        SceneManager.LoadScene("HexGrids", LoadSceneMode.Single);
    }

    

    public override bool IsDone()
    {
        return false;
    }

    private void SaveMechs()
    {
        Mech[] mechsToBeSaved = FindObjectsByType<Mech>(FindObjectsSortMode.InstanceID);
        foreach (Mech mech in mechsToBeSaved)
        {
            mech.transform.parent = safeObject.transform;
        }
    }
}