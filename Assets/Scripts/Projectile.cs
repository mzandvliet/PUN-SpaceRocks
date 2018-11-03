using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class Projectile : MonoBehaviour {
    [SerializeField] private float _shotImpulse = 20f;

    private PhotonView _view;
    private Rigidbody2D _body;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
        _body = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Start() {
        _body.AddRelativeForce(new Vector2(0f, _shotImpulse), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var rock = other.gameObject.GetComponent<SpaceRock>();

        if (rock != null) {
            var renderer = other.gameObject.GetComponent<MeshRenderer>();
            renderer.enabled = false;

            if (_view.IsMine) {
                ScoreManager.Instance.IncreaseScore(_view.Owner);

                // rock.View.RPC("DestroyRock", RpcTarget.All);
                PhotonNetwork.Destroy(rock.gameObject);
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
