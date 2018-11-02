using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


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
