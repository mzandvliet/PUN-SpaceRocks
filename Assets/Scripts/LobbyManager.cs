using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Photon.Pun.MonoBehaviourPunCallbacks {
    private ShipSpawner _shipSpawner;

    private void Awake() {
        _shipSpawner = GameObject.FindObjectOfType<ShipSpawner>();
    }

    private void Start() {
        var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
        _shipSpawner.SpawnShip(spawn.position, spawn.rotation);

        StatusGUI.Instance.SetStatus("Joined lobby, waiting...");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        StatusGUI.Instance.SetStatus("Player joined the game: " + newPlayer.NickName);

        if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(WaitAndStartGame());
        }
    }

    private IEnumerator WaitAndStartGame() {
        StatusGUI.Instance.SetStatus("Starting game in...");
        yield return new WaitForSeconds(1);
        for (int i = 5; i >= 1; i--) {
            StatusGUI.Instance.SetStatus(i + "...");
            yield return new WaitForSeconds(1);
        }
        StatusGUI.Instance.SetStatus("Launch!");
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnDisconnected(DisconnectCause cause) {
    }
}