using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class GameplayManager : Photon.Pun.MonoBehaviourPunCallbacks {
    [SerializeField] private SpaceRockSpawner _rockspawner;
    [SerializeField] private int _scoreTarget = 10;
    private ShipSpawner _shipSpawner;

    public event System.Action<string> OnMatchEnded;

    private void Awake() {
        _shipSpawner = GameObject.FindObjectOfType<ShipSpawner>();
    }

    private void Start() {
        StartCoroutine(PlayMatch());
    }

    public void LeaveMatch() {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("Launcher");
    }

    private IEnumerator PlayMatch() {
        // Initialize
        ScoreManager.Instance.ResetScores();

        var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
        _shipSpawner.SpawnShip(spawn.position, spawn.rotation);
        if (photonView.IsMine) {
            _rockspawner.StartSpawning();
        }

        // Loop
        while (true) {
            var players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++) {
                if (ScoreManager.Instance.GetScore(players[i]) >= _scoreTarget) {
                    StartCoroutine(EndGame(players[i]));
                    yield break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator EndGame(Player winner) {
        if (OnMatchEnded != null) {
            string winnerName = winner == PhotonNetwork.LocalPlayer ? "You" : winner.NickName;
            OnMatchEnded(winner.NickName);
        }

        StatusGUI.Instance.SetStatus(winner.NickName + " wins!");
        _rockspawner.StopSpawning();

        yield return new WaitForSeconds(5);

        for (int i = 8; i >= 1; i--) {
            StatusGUI.Instance.SetStatus("Moving on in..." + i);
            yield return new WaitForSeconds(1);
        }

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}