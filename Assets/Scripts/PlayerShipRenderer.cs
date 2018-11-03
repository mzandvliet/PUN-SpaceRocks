using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;


public class PlayerShipRenderer : MonoBehaviour {
    private PhotonView _view;
    private Renderer[] _renderers;

    private void Start() {
        _view = gameObject.GetComponent<PhotonView>();

        if (_view) {
            var color = Ramjet.Utilities.UnpackColor((int)_view.Owner.CustomProperties["shipColor"]);
            SetColor(color);
        }
    }

    public void SetColor(Color shipColor) {
        if (_renderers == null) {
            _renderers = gameObject.GetComponentsInChildren<Renderer>();
        }
        for (int i = 0; i < _renderers.Length; i++) {
            _renderers[i].material.SetColor("_Color", shipColor);
        }
    }
}
