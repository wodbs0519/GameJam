using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMaker : MonoBehaviour
{

	public Arrow arrow;

	private void Start()
	{
		MakeArrow();
	}

	public void MakeArrow()
	{
		var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 0.5f, 0));
		var _arrow = Instantiate(arrow, targetPos, Quaternion.identity, null);
		_arrow.MakeArrow += MakeArrow;
	}

}
