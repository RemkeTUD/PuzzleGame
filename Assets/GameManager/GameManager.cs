using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(Screen.currentResolution.refreshRate);
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
    private static PlayerController _player;
    public static PlayerController player
    {
        get
        {
            if (_player == null)
                _player = FindObjectOfType<PlayerController>();
            return _player;
        }
    }
}
