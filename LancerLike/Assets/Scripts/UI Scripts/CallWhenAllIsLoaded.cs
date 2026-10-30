using UnityEngine;
using UnityEngine.Events;

public class CallWhenAllIsLoaded : MonoBehaviour
{
    public static event UnityAction AllIsLoaded;
    public static void OnAllIsLoaded() => AllIsLoaded?.Invoke();


    void Start()
    {
        OnAllIsLoaded();
    }
}
