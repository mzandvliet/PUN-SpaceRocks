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

    I don't need to use the scene system if I don't want to.

    Todo: 
    - make a little statemachine to manage these flows
    - separate UI
 */

public class ConnectionManager : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IInRoomCallbacks {
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
		Debug.Log("ConnectionManager || Connecting...");
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


	#region IConnectionCallbacks

    public void OnConnected() {
        Debug.Log("ConnectionManager || Connected to server", this);

		
    }

    public void OnConnectedToMaster() {
        Debug.Log("ConnectionManager || Connected to master server", this);

		PhotonNetwork.GetCustomRoomList(TypedLobby.Default, "");

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnDisconnected(DisconnectCause cause) {
        Debug.Log("ConnectionManager || Disconnected, cause; " + cause, this);
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
    }

    public void OnJoinedRoom() {
        Debug.Log("ConnectionManager || Succesfully joined room");
    }

    public void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || JoinRoom failed");
    }

    public void OnJoinRandomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || JoinRandomRoom failed");

		CreateNewCustomRoom();
    }

    public void OnLeftRoom() {
        Debug.Log("ConnectionManager || Left room");
    }

    #endregion


    #region IInRoomCallbacks

    public void OnPlayerEnteredRoom(Player player) {
        Debug.Log("ConnectionManager || Player joined: " + player.NickName);
    }

    public void OnPlayerLeftRoom(Player player) {
        Debug.Log("ConnectionManager || Player left: " + player.NickName);
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
        Debug.Log("ConnectionManager || Some room properties changes, I dunno man...");
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        Debug.Log("ConnectionManager || Some player's properties changes, but why bother...");
    }

    public void OnMasterClientSwitched(Player newMasterClient) {
        Debug.Log("ConnectionManager || We're doing a host transfer, hold my beer...");
    }

    #endregion
}
