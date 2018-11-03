using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class PlayerShip : MonoBehaviour, IPunObservable {
    [SerializeField] private float _shotDelay = 0.33f;
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

    private void Start() {
        Color shipColor = Ramjet.Utilities.UnpackColor((int)_view.Owner.CustomProperties["shipColor"]);
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++) {
            renderers[i].material.SetColor("_Color", shipColor);
        }
    }

    private void Update() {
        if (_view.IsMine && Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.Time - _lastFireTime > _shotDelay) {
            Shoot();
            _lastFireTime = PhotonNetwork.Time;
        }
    }

    private void Shoot() {
        var pos = _transform.position + _transform.forward * 1.2f;
        pos.z = 0.5f;
        var rot = _transform.rotation;
        var projectile = PhotonNetwork.Instantiate(_projectilePrefab.name, pos, rot);
        var projectileBody = projectile.GetComponent<Rigidbody2D>();
        projectileBody.velocity = _body.GetPointVelocity(pos);
    }

    private void FixedUpdate() {
        if (!_view.IsMine) {
            return;
        }

        float turn = Input.GetAxis("Horizontal");
        float forward = Input.GetAxis("Vertical");

        float torque = turn * -150f * Time.fixedDeltaTime;
        float thrust = forward * 300f * Time.fixedDeltaTime;

        _body.angularDrag = 5f - 4.5f * Mathf.Abs(turn);

        _body.AddTorque(torque);
        _body.AddRelativeForce(new Vector2(0, thrust));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // Todo: Could perhaps improve client-side prediction by sending input
    }
}
