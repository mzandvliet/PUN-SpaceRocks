using System.Collections;
using System.Collections.Generic;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour {
    private static readonly string ScoreKey = "Score";

    private static ScoreManager _instance;
    private StringBuilder _string; // Used to minimize generate garbage for string concatination

    public static ScoreManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<ScoreManager>();
            }
            return _instance;
        }
    }

    private void Awake() {
        _string = new StringBuilder(1024);
    }

    private void OnGUI() {
        _string.Remove(0, _string.Length);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
            _string.Append(PhotonNetwork.PlayerList[i].NickName);
            _string.Append(": ");
            _string.Append(GetScore(PhotonNetwork.PlayerList[i]));
            _string.Append("\n");
        }
        GUILayout.Label(_string.ToString());
    }

    public void IncreaseScore(Player player) {
        int score = GetScore(player);
        player.CustomProperties[ScoreKey] = (object)(score + 1);
        player.SetCustomProperties(player.CustomProperties);
    }

    public int GetScore(Player player) {
        if (player.CustomProperties.ContainsKey(ScoreKey)) {
            return (int)player.CustomProperties[ScoreKey];
        }
        return 0;
    }
}
