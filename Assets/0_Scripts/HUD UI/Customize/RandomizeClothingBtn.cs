using System.Collections;
using System.Collections.Generic;
using PsychoticLab;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeClothingBtn : MonoBehaviour
{
    private bool _isRandomizing = false;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(RandomizeClothing);
    }

    private void RandomizeClothing()
    {
        _isRandomizing = !_isRandomizing;

        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<CharacterRandomizer>().enabled = _isRandomizing;
        }
    }
}