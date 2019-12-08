using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObj : MonoBehaviour
{

	public float speedCoefficient;
	private Transform _cam;
	private Vector3 prevPos;
	
	// Use this for initialization
	void Start () {
		_cam = Camera.main.transform;
		prevPos = _cam.position;
	}
	
	void Update () {
		transform.position -= ((prevPos - _cam.position)*speedCoefficient);
		prevPos = _cam.position;
	}
	
	
}
