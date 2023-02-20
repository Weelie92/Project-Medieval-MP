using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneList", menuName = "ScriptableObjects/SceneList", order = 1)]
public class SceneList : ScriptableObject
{
    [SerializeField] private Scene _lobbyScene;
    [SerializeField] private List<Scene> _minigameScenes;
}
