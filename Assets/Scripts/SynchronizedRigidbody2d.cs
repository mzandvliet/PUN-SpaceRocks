using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/* Todo:

    Asteroids follow predictable patterns of motion
    We can drastically lower the sync frequency for
    them, because determinism.


    Why do out-of-the-box Photon sync components
    both read and write when they are MINE?

 */


[RequireComponent(typeof(PhotonView), typeof(Rigidbody2D))]
public class SynchronizedRigidbody2d : MonoBehaviour, IPunObservable {
    private PhotonView _view;
    private Rigidbody2D _body;

    private Rigidbody2DState _extrapolatedServerState;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
        _body = gameObject.GetComponent<Rigidbody2D>();

        _extrapolatedServerState.position = _body.position;
        _extrapolatedServerState.rotation = _body.rotation;
        _extrapolatedServerState.velocity = _body.velocity;
        _extrapolatedServerState.angularVelocity = _body.angularVelocity;
    }

    private void FixedUpdate() {
        if (!_view.IsMine) {
            // Lerp 1st order state towards corrected server state
            _body.position = Vector3.Lerp(_body.position, _extrapolatedServerState.position, Time.fixedDeltaTime * 10f);
            _body.rotation = Mathf.Lerp(_body.rotation, _extrapolatedServerState.rotation, Time.fixedDeltaTime * 10f);
            // _body.velocity = Vector3.Lerp(_body.velocity, _extrapolatedServerState.velocity, Time.fixedDeltaTime * 10f);
            // _body.angularVelocity = Mathf.Lerp(_body.angularVelocity, _extrapolatedServerState.angularVelocity, Time.fixedDeltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (_view.IsMine && stream.IsWriting) {
            Vector2 pos = _body.position;
            Vector2 vel = _body.velocity;
            float rot = _body.rotation;
            float angVel = _body.angularVelocity;
            stream.Serialize(ref pos);
            stream.Serialize(ref vel);
            stream.Serialize(ref rot);
            stream.Serialize(ref angVel);
        }

        if (!_view.IsMine && stream.IsReading) {
            float latency = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));

            // Take server state, and predict forward in time by latency
            stream.Serialize(ref _extrapolatedServerState.position);
            stream.Serialize(ref _extrapolatedServerState.velocity);
            stream.Serialize(ref _extrapolatedServerState.rotation);
            stream.Serialize(ref _extrapolatedServerState.angularVelocity);

            _extrapolatedServerState.position += _extrapolatedServerState.velocity * latency;
            _extrapolatedServerState.rotation += _extrapolatedServerState.angularVelocity * latency;

            _body.velocity = _extrapolatedServerState.velocity;
            _body.angularVelocity = _extrapolatedServerState.angularVelocity;
        }
    }

    private struct Rigidbody2DState {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float angularVelocity;
    }
}
