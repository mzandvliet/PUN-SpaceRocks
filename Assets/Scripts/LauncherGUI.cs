using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Todo: 
    - make a little statemachine to manage these flows
 */

public class LauncherGUI : MonoBehaviour {
    [SerializeField] private ConnectionManager _connectionManager;
    
    [SerializeField] private InputField _callSignInputField;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private Button _launchButton;

    [SerializeField] private Text _statusText;
    [SerializeField] private Text _stateText;

    private static readonly string PrefsPlayerNicknameKey = "player/callsign";
    private static readonly string PrefsPlayerNicknameDefault = "Rook Trainee";

    private static readonly string PrefsPlayerShipColorKey = "player/shipcolor";
    private static readonly Color PrefsPlayerShipColorDefault = Color.white;


    private void Awake() {
        _callSignInputField.onValueChanged.AddListener(OnCallsignChanged);
        _colorPicker.onValueChanged.AddListener(OnColorChanged);
        _launchButton.onClick.AddListener(OnLaunchPressed);

        _connectionManager.OnNetworkEvent += OnNetworkEvent;
        _connectionManager.OnStateChanged += OnStateChanged;

        TestColorPacking();

        LoadPlayerSettings();
    }

    private static void TestColorPacking() {
        Color c = new Color(0.3f, 0.45f, 0.786f, 0.1f);
        int packed = Ramjet.Utilities.PackColor(c);
        Color unpacked = Ramjet.Utilities.UnpackColor(packed);

        Debug.Log(c);
        Debug.Log(packed);
        Debug.Log(unpacked);
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
        _connectionManager.SetPlayerNickname(_callSignInputField.text);
        _connectionManager.SetPlayerShipColor(_colorPicker.CurrentColor);
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
        if (!PlayerPrefs.HasKey(prefsKey)) {
            PlayerPrefs.SetInt(prefsKey, Ramjet.Utilities.PackColor(Color.white));
        }

        int packed = PlayerPrefs.GetInt(prefsKey);

        return Ramjet.Utilities.UnpackColor(packed);
    }

    private static void WriteColor(string prefsKey, Color color) {
        PlayerPrefs.SetInt(prefsKey, Ramjet.Utilities.PackColor(color));
    }

    
}
