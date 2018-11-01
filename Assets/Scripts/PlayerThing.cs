using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/*
    Oh my god this serialization scheme...
 */

[RequireComponent(typeof(PhotonView))]
public class PlayerThing : MonoBehaviour, IPunObservable {
    private PhotonView _view;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();
    }

    private void Update() {
        if (!_view.IsMine) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            _view.RPC(RPCID_AThingHappened, RpcTarget.Others, (byte)3);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            Vector3 pos = transform.position;
            stream.Serialize(ref pos);
        }

        if (stream.IsReading) {
            Vector3 pos = Vector3.zero;
            stream.Serialize(ref pos);
        }
    }

    public static readonly string RPCID_AThingHappened = "RPC_AThingHappened";
    [PunRPC]
    public void RPC_AThingHappened(byte theThing) {
        Debug.Log("A thing happened! " + theThing);
    }
}
