using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class LauncherManager : Photon.Pun.MonoBehaviourPunCallbacks {
    [SerializeField] private PunLogLevel _logLevel = PunLogLevel.Full;

    [SerializeField] private PlayerShipRenderer _playerShipRenderer;

    private static readonly string PrefsPlayerNicknameKey = "player/callsign";
    private static readonly string PrefsPlayerNicknameDefault = "Rook Trainee";

    private static readonly string PrefsPlayerShipColorKey = "player/shipcolor";
    private static readonly Color PrefsPlayerShipColorDefault = Color.white;

    private Color _shipColor;
    
    private void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.LogLevel = _logLevel;

        LoadPlayerSettings();
    }

    private void Start() {
        StatusGUI.Instance.SetStatus("Welcome! Please configure your craft. You may launch when ready.");
    }

    private void LoadPlayerSettings() {
        if (!PlayerPrefs.HasKey(PrefsPlayerNicknameKey)) {
            PlayerPrefs.SetString(PrefsPlayerNicknameKey, PrefsPlayerNicknameDefault);
        }

        PhotonNetwork.NickName = PlayerPrefs.GetString(PrefsPlayerNicknameKey, PrefsPlayerNicknameDefault);
        ShipColor = Ramjet.Utilities.ReadColorPrefs(PrefsPlayerShipColorKey);
    }

    public void Connect() {
        Debug.Log("ConnectionManager || Connecting...");
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void CreateNewRoom() {
        Debug.Log("ConnectionManager || Making a room...");
        string name = PhotonNetwork.LocalPlayer + "_" + System.DateTime.UtcNow;
        var options = new RoomOptions();
        options.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(name, options);
    }

    public string PlayerNickname {
        get { return PhotonNetwork.NickName; }
        set {
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(PrefsPlayerNicknameKey, value);
        }
    }


    public Color ShipColor {
        get { return _shipColor; }
        set {
            _shipColor = value;
            int packedColor = Ramjet.Utilities.PackColor(value);
            var props = PhotonNetwork.LocalPlayer.CustomProperties;
            if (!props.ContainsKey("shipColor")) {
                props.Add("shipColor", packedColor);
            }
            props["shipColor"] = packedColor;

            Ramjet.Utilities.WriteColorPrefs(PrefsPlayerShipColorKey, value);
            _playerShipRenderer.SetColor(value);
        }
        
    }

    #region PUN Callbacks

    public override void OnConnectedToMaster() {
        Debug.Log("ConnectionManager || Connected to master server", this);
        StatusGUI.Instance.SetStatus("Connected to master server");

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || JoinRandomRoom failed");

        CreateNewRoom();
    }

    public override void OnJoinedRoom() {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        Debug.LogError("ConnectionManager || JoinRoom failed");
    }

    public override void OnDisconnected(DisconnectCause cause) {
        
    }

    #endregion
}