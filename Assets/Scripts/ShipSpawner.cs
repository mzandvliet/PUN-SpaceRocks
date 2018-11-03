using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ShipSpawner : MonoBehaviour {
    [SerializeField] private GameObject _shipPrefab;
    [SerializeField] private CameraController _camera;

    private GameObject _shipInstance;

    public void SpawnShip(Vector3 position, Quaternion rotation) {
        _shipInstance = PhotonNetwork.Instantiate(_shipPrefab.name, position, rotation);
        var props = PhotonNetwork.LocalPlayer.CustomProperties;
       
        var color = Ramjet.Utilities.UnpackColor((int)PhotonNetwork.LocalPlayer.CustomProperties["shipColor"]);
        _shipInstance.GetComponent<PlayerShipRenderer>().SetColor(color);
        _camera.SetTarget(_shipInstance.GetComponent<Rigidbody2D>());
    }
}