using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/*
    Todo: 
    - make a little statemachine to manage these flows
    - separate UI
 */

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks {
	[SerializeField] private PunLogLevel _logLevel = PunLogLevel.Full;

	private void Awake() {
		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.AddCallbackTarget(this);
		PhotonNetwork.LogLevel = _logLevel;
	}

	private void OnDestroy() {
		Disconnect();
	}

    public void SetNickname(string nick) {
        PhotonNetwork.NickName = nick;
    }

	public void Connect() {
		Debug.Log("ConnectonManager || Connecting...");
		if (PhotonNetwork.IsConnected) {
			PhotonNetwork.JoinRandomRoom();
		} else {
			PhotonNetwork.ConnectUsingSettings();
		}
	}

	private void CreateNewCustomRoom() {
        Debug.Log("ConnectonManager || Making a room...");
		string name = "MySuperSpiffyRoom_" + System.DateTime.UtcNow;
		var options = new RoomOptions();
		options.MaxPlayers = 4;
		PhotonNetwork.CreateRoom(name, options);
	}

	private void Disconnect() {
        PhotonNetwork.Disconnect();
	}


	#region IConnectionCallbacks

    public void OnConnected() {
        Debug.Log("ConnectonManager || Connected to server", this);

		
    }

    public void OnConnectedToMaster() {
        Debug.Log("ConnectonManager || Connected to master server", this);

		PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnDisconnected(DisconnectCause cause) {
        Debug.Log("ConnectonManager || Disconnected, cause; " + cause, this);
    }

    public void OnRegionListReceived(RegionHandler regionHandler) {
        Debug.Log("ConnectonManager || Region list received: " + regionHandler.SummaryToCache, this);
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
        Debug.Log("ConnectonManager || Received custom auth response", this);

    }

    public void OnCustomAuthenticationFailed(string debugMessage) {
        Debug.Log("ConnectonManager || Custom auth failed: " + debugMessage, this);
    }

    #endregion


    #region IMatchmakingCallbacks

    public void OnFriendListUpdate(List<FriendInfo> friendList) {
    }

    public void OnCreatedRoom() {
        Debug.Log("ConnectonManager || Succesfully created room");
    }

    public void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogError("ConnectonManager || CreateRoom failed");
    }

    public void OnJoinedRoom() {
        Debug.Log("ConnectonManager || Succesfully joined room");
    }

    public void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError("ConnectonManager || JoinRoom failed");
    }

    public void OnJoinRandomFailed(short returnCode, string message) {
        Debug.LogError("ConnectonManager || JoinRandomRoom failed");

		CreateNewCustomRoom();
    }

    public void OnLeftRoom() {
        Debug.Log("ConnectonManager || Left room");
    }

    #endregion
}
