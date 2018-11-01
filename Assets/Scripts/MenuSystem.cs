using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Todo: 
    - make a little statemachine to manage these flows
 */

public class MenuSystem : MonoBehaviour {
    [SerializeField] private ConnectionManager _conMan;
    [SerializeField] private InputField _callSignInputField;
    [SerializeField] private Button _launchButton;

    [SerializeField] private Text _statusText;
    [SerializeField] private Text _stateText;

    private static readonly string PrefsPlayerNicknameKey = "player/callsign";
    private static readonly string PrefsPlayerNicknameDefault = "Rook Trainee";


    private void Awake() {
        _callSignInputField.onValueChanged.AddListener(OnCallsignChanged);
        _launchButton.onClick.AddListener(OnLaunchPressed);

        _conMan.OnNetworkEvent += OnNetworkEvent;
        _conMan.OnStateChanged += OnStateChanged;

        LoadPlayerSettings();
    }

    private void OnDestroy() {
        _callSignInputField.onValueChanged.RemoveListener(OnCallsignChanged);
        _launchButton.onClick.RemoveListener(OnLaunchPressed);

        _conMan.OnNetworkEvent -= OnNetworkEvent;
        _conMan.OnStateChanged -= OnStateChanged;
    }

    private void LoadPlayerSettings() {
        if (!PlayerPrefs.HasKey(PrefsPlayerNicknameKey)) {
            PlayerPrefs.SetString(PrefsPlayerNicknameKey, PrefsPlayerNicknameDefault);
        }

        string nick = PlayerPrefs.GetString(PrefsPlayerNicknameKey, PrefsPlayerNicknameDefault);
        _callSignInputField.text = nick;
    }

    private void OnCallsignChanged(string callsign) {
        PlayerPrefs.SetString(PrefsPlayerNicknameKey, callsign);
    }

    private void OnLaunchPressed() {
        _conMan.SetNickname(_callSignInputField.text);
        _conMan.Connect();
    }

    private void OnNetworkEvent(string evt) {
        _statusText.text = evt;
    }

    private void OnStateChanged(ConnectionState state) {
        _stateText.text = state.ToString();

        if (state == ConnectionState.Connecting) {
            _launchButton.gameObject.SetActive(false);
            _callSignInputField.gameObject.SetActive(false);
        }
    }
}
