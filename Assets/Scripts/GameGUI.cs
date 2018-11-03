using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour {
    [SerializeField] private GameplayManager _gameManager;
    [SerializeField] private GameObject _matchEndPanel;
    [SerializeField] private Button _leaveMatchButton;

    [SerializeField] private Text _winnerAnnounce;

    private void Awake() {
        _matchEndPanel.SetActive(false);
        _leaveMatchButton.onClick.AddListener(OnButtonPressed);
    }
    private void Start() {
        _gameManager.OnMatchEnded += OnMatchEnded;
        StatusGUI.Instance.SetStatus("Round started!");
    }

    private void OnMatchEnded(string winnerName) {
        _winnerAnnounce.text = winnerName + " won the match!";
        _matchEndPanel.SetActive(true);
    }

    private void OnButtonPressed() {
        _gameManager.LeaveMatch();
        _matchEndPanel.SetActive(false);
    }
}
