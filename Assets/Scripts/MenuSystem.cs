using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Todo: 
    - make a little statemachine to manage these flows
 */

public class MenuSystem : MonoBehaviour {
    [SerializeField] private ConnectionManager _connectionManager;
    
    [SerializeField] private InputField _callSignInputField;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private Button _launchButton;

    [SerializeField] private Text _statusText;
    [SerializeField] private Text _stateText;

    private static readonly string PrefsPlayerNicknameKey = "player/callsign";
    private static readonly string PrefsPlayerNicknameDefault = "Rook Trainee";

    private static readonly string PrefsPlayerShipColorKey = "player/shipcolor_";
    private static readonly Color PrefsPlayerShipColorDefault = Color.white;


    private void Awake() {
        _callSignInputField.onValueChanged.AddListener(OnCallsignChanged);
        _colorPicker.onValueChanged.AddListener(OnColorChanged);
        _launchButton.onClick.AddListener(OnLaunchPressed);

        _connectionManager.OnNetworkEvent += OnNetworkEvent;
        _connectionManager.OnStateChanged += OnStateChanged;

        LoadPlayerSettings();
    }

    private void OnDestroy() {
        _callSignInputField.onValueChanged.RemoveListener(OnCallsignChanged);
        _colorPicker.onValueChanged.RemoveListener(OnColorChanged);
        _launchButton.onClick.RemoveListener(OnLaunchPressed);

        _connectionManager.OnNetworkEvent -= OnNetworkEvent;
        _connectionManager.OnStateChanged -= OnStateChanged;
    }

    private void LoadPlayerSettings() {
        if (!PlayerPrefs.HasKey(PrefsPlayerNicknameKey)) {
            PlayerPrefs.SetString(PrefsPlayerNicknameKey, PrefsPlayerNicknameDefault);
        }

        string nick = PlayerPrefs.GetString(PrefsPlayerNicknameKey, PrefsPlayerNicknameDefault);
        _callSignInputField.text = nick;

        Color color = ReadColor(PrefsPlayerShipColorKey);
        _colorPicker.CurrentColor = color;
    }

    private void OnCallsignChanged(string callsign) {
        PlayerPrefs.SetString(PrefsPlayerNicknameKey, callsign);
    }

    private void OnColorChanged(Color color) {
        WriteColor(PrefsPlayerShipColorKey, color);
    }

    private void OnLaunchPressed() {
        _connectionManager.SetNickname(_callSignInputField.text);
        _connectionManager.Connect();
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


    private static Color ReadColor(string prefsKey) {
        if (!PlayerPrefs.HasKey(prefsKey + "r")) {
            PlayerPrefs.SetFloat(prefsKey + "r", 1f);
            PlayerPrefs.SetFloat(prefsKey + "g", 1f);
            PlayerPrefs.GetFloat(prefsKey + "b", 1f);
        }

        float r = PlayerPrefs.GetFloat(prefsKey + "r");
        float g = PlayerPrefs.GetFloat(prefsKey + "g");
        float b = PlayerPrefs.GetFloat(prefsKey + "b");

        return new Color(r,g,b,1f);
    }

    private static void WriteColor(string prefsKey, Color color) {
        PlayerPrefs.SetFloat(prefsKey + "r", color.r);
        PlayerPrefs.SetFloat(prefsKey + "g", color.g);
        PlayerPrefs.SetFloat(prefsKey + "b", color.b);
    }
}
