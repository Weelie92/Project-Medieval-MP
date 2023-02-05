using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameSettings : MonoBehaviour
{
    [SerializeField] string _minigameName;
    [SerializeField] string _minigameDescription;
    [SerializeField] int _timer;

    [Header("Minigame Team Size: Solo = FFA, Two = 2v2v2v2, Four = 4v4, 3 = 1vRest, All = All on team")]
    [SerializeField] private MinigameTeamSize _minigameTeamSize;



    enum MinigameTeamSize { Solo, Two, Four, OneMan, All };

}
