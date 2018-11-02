using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class PlayerShip : MonoBehaviour, IPunObservable {
    [SerializeField] private GameObject _projectilePrefab;

    private PhotonView _view;
    private Transform _transform;
    private Rigidbody2D _body;

    private double _lastFireTime = -1.0;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
        _transform = gameObject.GetComponent<Transform>();
        _body = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (_view.IsMine && Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.Time - _lastFireTime > 0.5) {
            var pos = _transform.position + _transform.forward * 1f;
            var rot = _transform.rotation;
            var projectile = PhotonNetwork.Instantiate(_projectilePrefab.name, pos, rot);
            var projectileBody = projectile.GetComponent<Rigidbody2D>();
            projectileBody.velocity = _body.GetPointVelocity(pos);
            
            _lastFireTime = PhotonNetwork.Time;
        }
    }

    private void FixedUpdate() {
        if (!_view.IsMine) {
            return;
        }

        float turn = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        float torque = turn * -140f * Time.fixedDeltaTime;
        float thrust = forward * 160f * Time.fixedDeltaTime;

        _body.angularDrag = 5f - 4.5f * Mathf.Abs(turn);

        _body.AddTorque(torque);
        _body.AddRelativeForce(new Vector2(0, thrust));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // Todo: Could perhaps improve client-side prediction by sending input
    }

    public static readonly string RPCID_AThingHappened = "RPC_AThingHappened";
    [PunRPC]
    public void RPC_AThingHappened(byte theThing) {
        Debug.Log("A thing happened! " + theThing);
    }
}
