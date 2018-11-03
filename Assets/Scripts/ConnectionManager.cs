﻿using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks {
	[SerializeField] private PunLogLevel _logLevel = PunLogLevel.Full;

    [SerializeField] private GameObject _shipPrefab;
    [SerializeField] private CameraController _camera;

    private GameObject _shipInstance;
    private ConnectionState _state;


    public event System.Action<ConnectionState> OnStateChanged;
    public event System.Action<string> OnNetworkEvent;

	private void Awake() {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_camera.gameObject);

		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.AddCallbackTarget(this);
		PhotonNetwork.LogLevel = _logLevel;
	}

    private void Start() {
        SetState(ConnectionState.Disconnected);
    }

	private void OnDestroy() {
		Disconnect();
	}

    public void SetPlayerNickname(string nick) {
        PhotonNetwork.NickName = nick;
    }

    public void SetPlayerShipColor(Color color) {
        int packedColor = Ramjet.Utilities.PackColor(color);
        var props = PhotonNetwork.LocalPlayer.CustomProperties;
        if (!props.ContainsKey("shipColor")) {
            props.Add("shipColor", packedColor);
        }
        props["shipColor"] = packedColor;
    }

	public void Connect() {
		Debug.Log("ConnectionManager || Connecting...");

        SetState(ConnectionState.Connecting);

		if (PhotonNetwork.IsConnected) {
			PhotonNetwork.JoinRandomRoom();
		} else {
			PhotonNetwork.ConnectUsingSettings();
		}
	}

    public void SpawnShip(Vector3 position, Quaternion rotation) {
        _shipInstance = PhotonNetwork.Instantiate(_shipPrefab.name, position, rotation);
        _camera.SetTarget(_shipInstance.GetComponent<Rigidbody2D>());
    }

	private void CreateNewCustomRoom() {
        Debug.Log("ConnectionManager || Making a room...");
		string name = PhotonNetwork.LocalPlayer + "_" + System.DateTime.UtcNow;
		var options = new RoomOptions();
		options.MaxPlayers = 4;
		PhotonNetwork.CreateRoom(name, options);
	}

	private void Disconnect() {
        PhotonNetwork.Disconnect();
	}

    private void SetState(ConnectionState state) {
        _state = state;
        if (OnStateChanged != null) {
            OnStateChanged(state);
        }
    }

    private void Notify(string msg) {
        if (OnNetworkEvent != null) {
            OnNetworkEvent(msg);
        }
    }

	#region IConnectionCallbacks

    public void OnConnected() {
        Debug.Log("ConnectionManager || Connected to server", this);
    }

    public void OnConnectedToMaster() {
        Debug.Log("ConnectionManager || Connected to master server", this);

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnDisconnected(DisconnectCause cause) {
        Debug.Log("ConnectionManager || Disconnected, cause; " + cause, this);

        SetState(ConnectionState.Disconnected);
    }

    public void OnRegionListReceived(RegionHandler regionHandler) {
        Debug.Log("ConnectionManager || Region list received: " + regionHandler.SummaryToCache, this);
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
        Debug.Log("ConnectionManager || Received custom auth response", this);

    }

    public void OnCustomAuthenticationFailed(string debugMessage) {
        Debug.Log("ConnectionManager || Custom auth failed: " + debugMessage, this);
    }

    #endregion


    #region IMatchmakingCallbacks

    public void OnFriendListUpdate(List<FriendInfo> friendList) {
    }

    public void OnCreatedRoom() {
        Debug.Log("ConnectionManager || Succesfully created room");
    }

    public void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || CreateRoom failed");
        OnNetworkEvent("Failed to create room...");
    }

    public void OnJoinedRoom() {
        Debug.Log("ConnectionManager || Succesfully joined room");

        // Transition to lobby

        PhotonNetwork.LoadLevel("Lobby");
        SetState(ConnectionState.Lobby);
    }

    public void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || JoinRoom failed");
        OnNetworkEvent("Failed to join room...");
    }

    public void OnJoinRandomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || JoinRandomRoom failed");

		CreateNewCustomRoom();
    }

    public void OnLeftRoom() {
        Debug.Log("ConnectionManager || Left room");

        // This is where the fun ends

        Destroy(_shipInstance);
    }

    #endregion


    #region IInRoomCallbacks

    public void OnPlayerEnteredRoom(Photon.Realtime.Player player) {
        Debug.Log("ConnectionManager || Player joined: " + player.NickName);
        OnNetworkEvent("Player joined the game: " + player.NickName);

        // Todo: count-down, transition to game
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel("Game");
            //_rockSpawner.StartSpawning();
        }
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player player) {
        Debug.Log("ConnectionManager || Player left: " + player.NickName);
        OnNetworkEvent("Player left the game: " + player.NickName);
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
        Debug.Log("ConnectionManager || Some room properties changes, I dunno man...");
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        Debug.Log("ConnectionManager || Some player's properties changes, but why bother...");
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) {
        Debug.Log("ConnectionManager || We're doing a host transfer, hold my beer...");
    }

    #endregion
}

public enum ConnectionState {
    Disconnected,
    Connecting,
    Lobby,
    Playing
}