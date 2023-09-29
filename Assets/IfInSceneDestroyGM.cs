using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfInSceneDestroyGM : MonoBehaviour
{
    // Singleton instance of the object
    public static IfInSceneDestroyGM Instance { get; private set; }

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
