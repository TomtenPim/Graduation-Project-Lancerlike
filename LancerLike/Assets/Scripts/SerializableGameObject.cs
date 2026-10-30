using UnityEngine;

public class SaveObject:MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}