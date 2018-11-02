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

    public static ScoreManager Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<ScoreManager>();
            }
            return _instance;
        }
    }

    private void Awake() {
        
    }

    private void OnGUI() {
        StringBuilder sb = new StringBuilder("Scores:\n");
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) {
            sb.Append(PhotonNetwork.PlayerList[i].NickName + ": " + GetScore(PhotonNetwork.PlayerList[i]) + "\n");
        }

        GUILayout.Label(sb.ToString());
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
        return -1;
    }
}
