using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour {
    private ConnectionManager _connectionManager;

    private void Awake() {
        _connectionManager = FindObjectOfType<ConnectionManager>();
    }

    private void Start() {
        if (_connectionManager) {
            var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
            _connectionManager.SpawnShip(spawn.position, spawn.rotation);
        } else {
            Debug.LogWarning("ConnectionManager not found...");
        }
    }
}