using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameplayManager : MonoBehaviour {
    [SerializeField] private SpaceRockSpawner _rockspawner;
    
    private ConnectionManager _connectionManager;


    private void Awake() {
        _connectionManager = FindObjectOfType<ConnectionManager>();
    }

    private void Start() {
        if (_connectionManager) {
            var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
            _connectionManager.SpawnShip(spawn.position, spawn.rotation);
        } else {
            Debug.LogWarning("ConnectionManager not found...");
        }

        _rockspawner.StartSpawning();
    }
}