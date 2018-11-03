using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : Photon.Pun.MonoBehaviourPunCallbacks {
    private ShipSpawner _shipSpawner;

    private Coroutine _waitRoutine;

    private void Awake() {
        _shipSpawner = GameObject.FindObjectOfType<ShipSpawner>();
    }

    private void Start() {
        var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
        _shipSpawner.SpawnShip(spawn.position, spawn.rotation);

        StatusGUI.Instance.SetStatus("Joined lobby, waiting for others...");

        _waitRoutine = StartCoroutine(WaitForPlayers(2));
    }

    private IEnumerator WaitForPlayers(int minimum) {
        bool waiting = true;
        while (waiting) {
            Debug.Log("Waiting on players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + minimum);
            yield return new WaitForSeconds(1f);
            if (PhotonNetwork.CurrentRoom.PlayerCount >= minimum) {
                if (PhotonNetwork.IsMasterClient) {
                    photonView.RPC("RPC_StartMatch", RpcTarget.All, PhotonNetwork.Time);
                    waiting = false;
                }
            }
        }
    }

    [PunRPC]
    public void RPC_StartMatch(double serverTimestamp) {
        float latency = Mathf.Abs((float)(PhotonNetwork.Time - serverTimestamp));
        StopCoroutine(_waitRoutine);
        StartCoroutine(AnnounceAndStartGame(latency));
    }

    private IEnumerator AnnounceAndStartGame(float latency) {
        StatusGUI.Instance.SetStatus("Starting game in...");

        yield return new WaitForSeconds(Mathf.Max(0f, 2f - latency));

        for (int i = 5; i >= 1; i--) {
            StatusGUI.Instance.SetStatus(i + "...");
            yield return new WaitForSeconds(1);
        }
        StatusGUI.Instance.SetStatus("Launch!");
        yield return new WaitForSeconds(1);

        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("Game");
        }
    }


    public override void OnDisconnected(DisconnectCause cause) {
    }

    public override void OnPlayerEnteredRoom(Player player) {
        StatusGUI.Instance.SetStatus("Player joined the lobby: " + player.NickName);
    }

    public override void OnPlayerLeftRoom(Player player) {
        StatusGUI.Instance.SetStatus("Player left the lobby: " + player.NickName);
    }
}