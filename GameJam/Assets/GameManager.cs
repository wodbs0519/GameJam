using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;
	
	private Player _player;
	private void Start()
	{
		instance = this;
		_player = Player.instance;
	}

	public void Load()
	{
		var targetPos = new Vector2(PlayerPrefs.GetFloat("Vector2.x"), PlayerPrefs.GetFloat("Vector2.y"));
		_player.transform.position = targetPos;
		_player.health = _player.StartHealth;
	}
	
}
