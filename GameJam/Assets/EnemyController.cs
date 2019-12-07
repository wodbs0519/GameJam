using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
	public EnemyBase enemy;

	public float minRespawnTime;
	public float maxRespawnTime;

	private Camera _camera;

	private void Start()
	{
		_camera = Camera.main;
		MakeNewEnemy();
	}

	private void MakeNewEnemy()
	{
		StartCoroutine(DealySpawn());
	}

	IEnumerator DealySpawn()
	{
		yield return new WaitForSeconds(Random.Range(minRespawnTime, maxRespawnTime));
		
		var targetPoint = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height * 0.5f));
		var newEnemy = Instantiate(enemy, targetPoint, Quaternion.identity);
		newEnemy.deadCallback += MakeNewEnemy;
	}
	
}

