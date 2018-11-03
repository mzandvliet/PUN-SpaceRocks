using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class SpaceRockSpawner : MonoBehaviour {
    [SerializeField] private float _spawnPeroid = 2f;
    [SerializeField] private GameObject _rockPrefab;

    private Coroutine _spawnRoutine;

    public void StartSpawning() {
        _spawnRoutine = StartCoroutine(SpawnRepeating());
    }

    public void StopSpawning() {
        if (_spawnRoutine != null) {
            StopCoroutine(_spawnRoutine);
        }
    }

    private IEnumerator SpawnRepeating() {
        while (true) {
            var rock = PhotonNetwork.Instantiate(_rockPrefab.name, RandomOnCircle(30f), Quaternion.AngleAxis(UnityEngine.Random.Range(0f, Mathf.PI * 2f), Vector3.up));
            var body = rock.GetComponent<Rigidbody2D>();
            var velocity = (-body.position.normalized + RandomOnCircle(0.5f)) * Random.Range(1f, 3f);
            var angularVelocity = Random.Range(-Mathf.PI * 0.5f, Mathf.PI * 0.5f);
            body.velocity = velocity;
            body.angularVelocity = angularVelocity;

            yield return new WaitForSeconds(_spawnPeroid);
        }
    }

    private static Vector2 RandomOnCircle(float radius) {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
    }
}
