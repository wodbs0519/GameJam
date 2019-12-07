using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEnemy : MonoBehaviour
{

	public EnemyBase enemy;
	public Transform RespawnPoint;
	private Collider2D _collider;

	private void Start()
	{
		_collider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter(Collider other)
	{
		Instantiate(enemy, RespawnPoint.position, Quaternion.identity);
		_collider.enabled = false;
	}
}
