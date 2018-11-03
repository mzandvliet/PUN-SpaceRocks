using UnityEngine;
using UnityEngine.UI;

public class StatusGUI : MonoBehaviour {
    [SerializeField] private Text _statusText;
    [SerializeField] private Text _stateText;
    
    private static StatusGUI _instance;
    public static StatusGUI Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<StatusGUI>();
            }
            return _instance;
        }
    }

    public void SetStatus(string text) {
        _statusText.text = text;
    }

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}