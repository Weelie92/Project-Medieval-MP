using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfInSceneDestroyUI : MonoBehaviour
{
    // Singleton instance of the object
    public static IfInSceneDestroyUI Instance { get; private set; }

    // Awake is called before Start
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
