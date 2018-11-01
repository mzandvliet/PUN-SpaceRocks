using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/*

    Things I don't like about Photon:
    - Every object responsible for synchronizing itself
        - Trashes cache because of discontinuous traversal
        - Objects typically do lots of branching to figure out context
    - Code for Server/Client/Owner/Remote roles is mushed together, needs lots of branching based on view.isMine
    - Peer2Peer by default, which makes it absolutely trivial to write cheats and exploits

    Need to make this a prefab, spawn it through Photon, I think.
    Look at how it buffers stuff.
 */

[RequireComponent(typeof(PhotonView))]
public class PlayerShip : MonoBehaviour, IPunObservable {
    private PhotonView _view;
    private Rigidbody2D _body;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
        _body = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (!_view.IsMine) {
            return;
        }

        float turn = Input.GetAxis("Horizontal") * -20f * Time.fixedDeltaTime;
        float thrust = Input.GetAxis("Vertical") * 80f * Time.fixedDeltaTime;

        _body.AddTorque(turn);
        _body.AddRelativeForce(new Vector2(0, thrust));
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
            Vector2 pos = Vector3.zero;
            Vector2 vel = Vector3.zero;
            float rot = _body.rotation;
            float angVel = _body.angularVelocity;
            stream.Serialize(ref pos);
            stream.Serialize(ref vel);
            stream.Serialize(ref rot);
            stream.Serialize(ref angVel);
            _body.position = pos;
            _body.velocity = vel;
            _body.rotation = rot;
            _body.angularVelocity = angVel;
        }
    }

    public static readonly string RPCID_AThingHappened = "RPC_AThingHappened";
    [PunRPC]
    public void RPC_AThingHappened(byte theThing) {
        Debug.Log("A thing happened! " + theThing);
    }
}
