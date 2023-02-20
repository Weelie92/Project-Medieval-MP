using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _score;

    public void SetPlayerScore(ulong playerId, int score)
    {
        

        _playerName.text = "Player: " + playerId.ToString();
        _score.text = score.ToString();
    }
}
