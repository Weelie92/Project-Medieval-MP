using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;

    [SerializeField] private GameObject _scoreBoardContentPrefab;
    [SerializeField] private Transform _scoreBoardContentParent;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        BuildScoreBoard();
    }

    public void BuildScoreBoard()
    {
        ClearScoreBoard();

        Dictionary<ulong, int> list = MinigameScoreTracker.Instance.GetPlayerScores();

        var sortedPlayerScores = list.OrderByDescending(kvp => kvp.Value);



        for (int i = 0; i < list.Count; i++)
        {
            {
                KeyValuePair<ulong, int> pair = sortedPlayerScores.ElementAt(i);

                ulong clientId = pair.Key;
                int score = pair.Value;

                GameObject scoreBoardContent = Instantiate(_scoreBoardContentPrefab, _scoreBoardContentParent);

                scoreBoardContent.GetComponent<PlayerScoreUI>().SetPlayerScore(clientId, score);
            }
        }
    }

    private void ClearScoreBoard()
    {
        foreach (Transform child in _scoreBoardContentParent)
        {
            Destroy(child.gameObject);
        }
        
    }
}
