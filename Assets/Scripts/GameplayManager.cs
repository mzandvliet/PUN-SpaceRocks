using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
        StopAllCoroutines();
        PhotonNetwork.DestroyAll(true);
        PhotonNetwork.Disconnect();
        StartCoroutine(WaitAndReturnToLauncher());
    }

    private IEnumerator WaitAndReturnToLauncher() {
        // This because LoadScene in same frame of disconnect causes issues
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Launcher");
    }

    private IEnumerator PlayMatch() {
        // Initialize
        ScoreManager.Instance.ResetScores();
        SpawnMyShip();

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

    private void OnShipDeath(PlayerShip ship) {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(3f);
        SpawnMyShip();
    }

    private void SpawnMyShip() {
        var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
        var ship = _shipSpawner.SpawnShip(spawn.position, spawn.rotation);
        ship.OnDeath += OnShipDeath;
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
            StatusGUI.Instance.SetStatus("Next game starting in..." + i);
            yield return new WaitForSeconds(1);
        }

        if (PhotonNetwork.IsMasterClient) {
            // Take everyone who's left to the lobby
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
}