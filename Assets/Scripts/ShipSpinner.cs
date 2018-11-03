using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpinner : MonoBehaviour {
	private Rigidbody2D _body;

	void Start () {
		_body = gameObject.GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
		_body.AddTorque(4f * Time.fixedDeltaTime);
		if (Input.GetKey(KeyCode.Mouse0)) {
            	_body.AddTorque(Input.GetAxis("Mouse X") * -100f * Time.fixedDeltaTime);
		}
	}
}
