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

        InvokeRepeating(nameof(UpdateScoreBoard), 5f, 5f);
    }

    void UpdateScoreBoard()
    {
        BuildScoreBoard();
    }

    public void BuildScoreBoard()
    {
        ClearScoreBoard();

        Dictionary<ulong, int> scoreList = MinigameScoreTracker.Instance.GetPlayerScores();
        Dictionary<ulong, string> nameList = MinigameScoreTracker.Instance.GetPlayerNames();

        var sortedPlayerScores = scoreList.OrderByDescending(kvp => kvp.Value);

        for (int i = 0; i < scoreList.Count; i++)
        {
            KeyValuePair<ulong, int> scorePair = sortedPlayerScores.ElementAt(i);

            ulong clientId = scorePair.Key;
            int score = scorePair.Value;
            string name = nameList[clientId];

            GameObject scoreBoardContent = Instantiate(_scoreBoardContentPrefab, _scoreBoardContentParent);

            scoreBoardContent.GetComponent<PlayerScoreUI>().SetPlayerScore(name, score);
        }
    }


    private void ClearScoreBoard()
    {
        if (_scoreBoardContentParent == null) _scoreBoardContentParent = GameObject.FindGameObjectWithTag("UI_ScoreBoardBG").transform;
        foreach (Transform child in _scoreBoardContentParent)
        {
            Destroy(child.gameObject);
        }
        
    }
}
