using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/* 
    Rudimentary synchronization for 2d rigidbodies.

    With more time I would:
    - Add acceleration as a factor to sync (essentially, user inputs)
 */

[RequireComponent(typeof(PhotonView), typeof(Rigidbody2D))]
public class SynchronizedRigidbody2d : MonoBehaviour, IPunObservable {
    [SerializeField] private bool _synchRotation;

    private PhotonView _view;
    private Rigidbody2D _body;

    private Rigidbody2DState _serverState;
    private bool _initialized;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
        _body = gameObject.GetComponent<Rigidbody2D>();

        _serverState.position = _body.position;
        _serverState.rotation = _body.rotation;
        _serverState.velocity = _body.velocity;
        _serverState.angularVelocity = _body.angularVelocity;
    }

    private void FixedUpdate() {
        if (!_view.IsMine) {
            // Lerp 1st order state towards corrected server state
            _body.position = Vector3.Lerp(_body.position, _serverState.position, Time.fixedDeltaTime * 10f);

            if (_synchRotation) {
                _body.rotation = Mathf.Lerp(_body.rotation, _serverState.rotation, Time.fixedDeltaTime * 10f);
            }
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
            if (_synchRotation) {
                stream.Serialize(ref rot);
                stream.Serialize(ref angVel);
            }
        }

        if (!_view.IsMine && stream.IsReading) {
            float latency = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));

            stream.Serialize(ref _serverState.position);
            stream.Serialize(ref _serverState.velocity);

            // Take server state, and predict forward in time by latency
            _serverState.position += _serverState.velocity * latency;
            _body.velocity = _serverState.velocity;

            if (_synchRotation) {
                stream.Serialize(ref _serverState.rotation);
                stream.Serialize(ref _serverState.angularVelocity);
                // We set 2nd order state directly, should barely make a visual difference
                _serverState.rotation += _serverState.angularVelocity * latency;
                _body.angularVelocity = _serverState.angularVelocity;
            }

            if (!_initialized) {
                // Snap to server state on first sync
                // Todo: Not needed if we send initial state as part of the instantiation call, how does photon let us do that?
                _body.position = _serverState.position;
                _body.velocity = _serverState.velocity;
                _body.rotation = _serverState.rotation;
                _body.angularVelocity = _serverState.angularVelocity;
                _initialized = true;
            }
        }
    }

    private struct Rigidbody2DState {
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float angularVelocity;
    }
}
