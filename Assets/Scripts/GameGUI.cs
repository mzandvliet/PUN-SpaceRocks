using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Todo: 
    - make a little statemachine to manage these flows
 */

public class GameGUI : MonoBehaviour {
    [SerializeField] private Text _statusText;
    [SerializeField] private Text _stateText;

    private static readonly string PrefsPlayerNicknameKey = "player/callsign";
    private static readonly string PrefsPlayerNicknameDefault = "Rook Trainee";

    private static readonly string PrefsPlayerShipColorKey = "player/shipcolor";
    private static readonly Color PrefsPlayerShipColorDefault = Color.white;

    private void OnNetworkEvent(string evt) {
        _statusText.text = evt;
    }
}
