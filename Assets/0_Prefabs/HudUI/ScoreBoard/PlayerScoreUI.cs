using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerScoreUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _score;

    public void SetPlayerScore(string playerName, int score)
    {
        _playerName.text = playerName;
        _score.text = score.ToString();
    }
}