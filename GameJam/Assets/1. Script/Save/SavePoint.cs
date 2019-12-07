using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player")) return;
		
		var position = Player.instance.transform.position;
		PlayerPrefs.SetFloat("Vector2.x", position.x);
		PlayerPrefs.SetFloat("Vector2.y", position.y);
		Destroy(gameObject);
	}
}
