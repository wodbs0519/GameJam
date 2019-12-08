using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakuraUI : MonoBehaviour
{

	private Animator anim;

	private void Start()
	{
		anim = GetComponent<Animator>();
	}

	public void Damaged()
	{
		anim.Play("Damaged", 0, 0);
	}

	public void Recovery()
	{
		anim.Play("Recovery", 0, 0);
	}

}
