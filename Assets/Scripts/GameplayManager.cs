using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameplayManager : Photon.Pun.MonoBehaviourPunCallbacks {
    [SerializeField] private SpaceRockSpawner _rockspawner;
    private ShipSpawner _shipSpawner;


    private void Awake() {
        _shipSpawner = GameObject.FindObjectOfType<ShipSpawner>();
    }

    private void Start() {
        var spawn = Ramjet.Utilities.GetSpawnLocation(PhotonNetwork.LocalPlayer.ActorNumber);
        _shipSpawner.SpawnShip(spawn.position, spawn.rotation);
        _rockspawner.StartSpawning();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        
    }
}