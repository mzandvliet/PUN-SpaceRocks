using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	private Transform _transform;
	private Rigidbody2D _target;

	private void Awake() {
		_transform = gameObject.GetComponent<Transform>();
	}

	public void SetTarget(Rigidbody2D target) {
		_target = target;
		if (_target != null) {
            _transform.position = target.position;
		}
	}
	
	void Update () {
		if (_target == null) {
			return;
		}

		Vector3 p = _target.position;

		Vector3 newPosition = Vector3.Lerp(_transform.position, p, 2f * Time.deltaTime * Vector3.Distance(p, _transform.position));
		newPosition.z = -1f;
		_transform.position = newPosition;
	}
}
