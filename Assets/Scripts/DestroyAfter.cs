using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(PhotonView))]
public class DestroyAfter : MonoBehaviour {
    [SerializeField] private float _lifeTime = 10;
    
    private PhotonView _view;

    private void Awake() {
        _view = gameObject.GetComponent<PhotonView>();

        if (_view.IsMine) {
            StartCoroutine(Destroy(_lifeTime));
        }
    }

    private IEnumerator Destroy(float time) {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(_view);
    }
}
