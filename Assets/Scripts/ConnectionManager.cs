using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/*

    Can use PhotonNetwork.LoadLevel() when PhotonNetwork.isMasterClient. If AutoSyncScene
    is set, then all connected clients will follow. Basically, am I a server for this room?

    Need a lobby / solo-warmup stage

 */

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks {
	[SerializeField] private PunLogLevel _logLevel = PunLogLevel.Full;

    [SerializeField] private GameObject _shipPrefab;
    [SerializeField] private CameraController _camera;
    [SerializeField] private SpaceRockSpawner _rockSpawner;

    private GameObject _shipInstance;
    private ConnectionState _state;


    public event System.Action<ConnectionState> OnStateChanged;
    public event System.Action<string> OnNetworkEvent;

	private void Awake() {
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

	private void CreateNewCustomRoom() {
        Debug.Log("ConnectionManager || Making a room...");
		string name = "MySuperSpiffyRoom_" + System.DateTime.UtcNow;
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

        SetState(ConnectionState.Connected);
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

        // This is where we actually start playing

        _shipInstance = PhotonNetwork.Instantiate(_shipPrefab.name, Vector3.zero, Quaternion.identity); // Todo: can add color here
        _camera.SetTarget(_shipInstance.GetComponent<Rigidbody2D>());
        _rockSpawner.StartSpawning();

        SetState(ConnectionState.Playing);
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

        _rockSpawner.StopSpawning();
        Destroy(_shipInstance);
    }

    #endregion


    #region IInRoomCallbacks

    public void OnPlayerEnteredRoom(Photon.Realtime.Player player) {
        Debug.Log("ConnectionManager || Player joined: " + player.NickName);
        OnNetworkEvent("Player joined the game: " + player.NickName);
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
    Connected,
    Playing
}