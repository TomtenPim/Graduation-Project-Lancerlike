using Actions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class MechCustmizationAction : ActionStack.ActionBehavior
{
    private Mech currentMech;
    bool isDoneCustomizing = false;
    [SerializeField] MountHolder mountHolder; 
    private void Start()
    {
        mountHolder.SetUp(currentMech);
    }

    public void OnDone()
    {
        isDoneCustomizing = true;
    }

    public override bool IsDone()
    {
        return isDoneCustomizing;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        Destroy(gameObject);      // commit greater sepuku!
    }

    public static MechCustmizationAction Create(Transform parent, Mech activeMech)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/MechCustomizationAction");
        GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent, false);
        go.transform.position = new Vector3(go.transform.position.x - parent.transform.position.x, go.transform.position.y - parent.transform.position.y, go.transform.position.z);

        go.GetComponent<MechCustmizationAction>().currentMech = activeMech;
        return go.GetComponent<MechCustmizationAction>();
    }
}