using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;


public class PlayerShipRenderer : MonoBehaviour {
    private Renderer[] _renderers;

    public void SetColor(Color shipColor) {
        if (_renderers == null) {
            _renderers = gameObject.GetComponentsInChildren<Renderer>();
        }
        for (int i = 0; i < _renderers.Length; i++) {
            _renderers[i].material.SetColor("_Color", shipColor);
        }
    }
}
