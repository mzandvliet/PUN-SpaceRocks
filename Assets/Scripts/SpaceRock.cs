using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class SpaceRock : MonoBehaviour {
    private PhotonView _view;
    private Rigidbody2D _body;

    public PhotonView View {
        get { return _view; }
    }

    public double SpawnTime {
        get;
        set;
    }

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
        _body = gameObject.GetComponent<Rigidbody2D>();
    }

    [PunRPC]
    public void DestroyRock() {
        if (_view.IsMine) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
