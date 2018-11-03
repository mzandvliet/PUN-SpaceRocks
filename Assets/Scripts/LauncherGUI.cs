using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Todo: 
    - make a little statemachine to manage these flows
 */

public class LauncherGUI : MonoBehaviour {
    [SerializeField] private LauncherManager _launcher;
    [SerializeField] private InputField _callSignInputField;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private Button _launchButton;

    private void Start() {
        _callSignInputField.text = _launcher.PlayerNickname;
        _colorPicker.CurrentColor = _launcher.ShipColor;

        _callSignInputField.onValueChanged.AddListener(OnCallsignChanged);
        _colorPicker.onValueChanged.AddListener(OnColorChanged);
        _launchButton.onClick.AddListener(OnLaunchPressed);
    }

    private void OnDestroy() {
        _callSignInputField.onValueChanged.RemoveListener(OnCallsignChanged);
        _colorPicker.onValueChanged.RemoveListener(OnColorChanged);
        _launchButton.onClick.RemoveListener(OnLaunchPressed);
    }

    private void OnCallsignChanged(string callsign) {
        _launcher.PlayerNickname = _callSignInputField.text;
    }

    private void OnColorChanged(Color color) {
        _launcher.ShipColor = _colorPicker.CurrentColor;
    }

    private void OnLaunchPressed() {
        _launcher.Connect();

        _launchButton.gameObject.SetActive(false);
        _callSignInputField.gameObject.SetActive(false);
        _colorPicker.gameObject.SetActive(false);
    }

    private static void TestColorPacking() {
        Color c = new Color(0.3f, 0.45f, 0.786f, 0.1f);
        int packed = Ramjet.Utilities.PackColor(c);
        Color unpacked = Ramjet.Utilities.UnpackColor(packed);

        Debug.Log(c);
        Debug.Log(packed);
        Debug.Log(unpacked);
    }
}
