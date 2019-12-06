using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{

	public float attackDelay;
	public float attackDamage;
	public float runDist;
	public float attackDist;
	public float walkSpeed;
	public float runSpeed;
	public float bashAddition;

	public EnemyType Type;
	
	public int Health
	{
		get { return _health; }
		set
		{
			_health = value;
			if (_health <= 0)
			{
				Dead();
			}
		}
	}

	private int _health = 3;
	private bool _isAttack;
	private Vector2 _targetDir;
	private Rigidbody2D _rigidbody;
	private Animator _animator;
	private SpriteRenderer _renderer;

	public Transform Target;


	private void Start()
	{
		Target = Player.instance.transform;
		_isAttack = false;
		_rigidbody = GetComponent<Rigidbody2D>();
		_renderer = GetComponentInChildren<SpriteRenderer>();
		_animator = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		Vector2 targetVector = Target.position - transform.position;
		_targetDir = targetVector.normalized;
		if (_targetDir.x > 0)
		{
			_renderer.flipX = true;
		}
		else
		{
			_renderer.flipX = false;
		}
		
		float dist = Vector2.SqrMagnitude(targetVector);
		if (dist > runDist * runDist)
		{
			Walk();
		}
		else if (dist > attackDist * attackDist)
		{
			Run();
		}
		else
		{
			StartAttack();
		}
	}

	private void Walk()
	{
		var currentVelocity = _rigidbody.velocity;
		currentVelocity.x = walkSpeed * Mathf.Sign(_targetDir.x);
		_rigidbody.velocity = currentVelocity;
	}
	
	private void Run()
	{
		var currentVelocity = _rigidbody.velocity;
		currentVelocity.x = runSpeed * Mathf.Sign(_targetDir.x);
		_rigidbody.velocity = currentVelocity;
	}
	

	public void StartAttack()
	{
		if (_isAttack) return;
		_isAttack = true;
		StartCoroutine(Attack());
	}

	IEnumerator Attack()
	{
		yield return new WaitForSeconds(attackDelay);
		Debug.Log("Attack!");
		Player.instance.AttackCheck(_targetDir, attackDamage, Type);
		_isAttack = false;
	}
	


	private void Dead()
	{
		Debug.Log("I'm Dead");
		DestroyImmediate(gameObject);
	}

	public void Damaged(int damage)
	{
		Health -= damage;
		StopAllCoroutines();
		Bashed();
	}

	public void Bashed()
	{
		_rigidbody.AddForce(Vector2.right * -_targetDir * bashAddition);
	}
	
	
}

public enum EnemyType
{
	Small,
	Middle,
	Large
}
