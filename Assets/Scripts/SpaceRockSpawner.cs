using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class SpaceRockSpawner : MonoBehaviour {
    [SerializeField] private float _rockLifetime = 30f;
    [SerializeField] private float _spawnPeroid = 2f;
    [SerializeField] private GameObject _rockPrefab;

    private Coroutine _spawnRoutine;

    private List<SpaceRock> _rocks;

    private void Awake() {
        _rocks = new List<SpaceRock>();
    }

    public void StartSpawning() {
        _spawnRoutine = StartCoroutine(SpawnRepeating());
    }

    public void StopSpawning() {
        if (_spawnRoutine != null) {
            StopCoroutine(_spawnRoutine);
        }

        for (int i = 0; i < _rocks.Count; i++) {
            if (_rocks[i] != null) {
                PhotonNetwork.Destroy(_rocks[i].gameObject);
            }
        }
        _rocks.Clear();
    }

    private IEnumerator SpawnRepeating() {
        while (true) {
           
            for (int i = _rocks.Count-1; i >= 0; i--) {
                if (_rocks[i] != null) {
                    // Clean up old rocks
                    if (_rocks[i].SpawnTime + _rockLifetime < PhotonNetwork.Time) {
                        PhotonNetwork.Destroy(_rocks[i].gameObject);
                        _rocks.RemoveAt(i);
                    }
                } else {
                    // Remove entries for rocks destroyed by projectiles
                    _rocks.RemoveAt(i);
                }
            }

            // Spawn a new one

            var obj = PhotonNetwork.Instantiate(_rockPrefab.name, RandomOnCircle(30f), Quaternion.AngleAxis(UnityEngine.Random.Range(0f, Mathf.PI * 2f), Vector3.up));
            
            var body = obj.GetComponent<Rigidbody2D>();
            var velocity = (-body.position.normalized + RandomOnCircle(0.5f)) * Random.Range(1f, 3f);
            var angularVelocity = Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f);
            body.velocity = velocity;
            body.angularVelocity = angularVelocity;

            var rock = obj.GetComponent<SpaceRock>();
            rock.SpawnTime = PhotonNetwork.Time;

            _rocks.Add(rock);

            yield return new WaitForSeconds(_spawnPeroid);
        }
    }

    private static Vector2 RandomOnCircle(float radius) {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }
}
