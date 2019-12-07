using System;
using UnityEngine;

public class Player : MonoBehaviour
{

	public static Player instance;

	public float StartHealth;
	
	public float health
	{
		get { return _health; }
		set
		{
			_health = value;
			if (_health <= 0)
			{
				Debug.Log("Player Dead");
				OnPlayerDead.Raise();
			}
		}
	}

	public GameEvent OnPlayerDead;

	public float attackRadius;
	public float perfectAttackRadius;
	public LayerMask attackLayer;
	public int counterDamage;
	public int perfectCounterDamage;
	private float _health;
	private Controller _controller;

	private void Awake()
	{
		instance = this;
		_controller = GetComponent<Controller>();
		health = StartHealth;
	}

	// Update is called once per frame
	void Update () {
		var moveDir = Input.GetAxisRaw("Horizontal");
		_controller.Move(moveDir);

		if (Input.GetKeyDown(KeyCode.Z))
		{
			_controller.StartCounter();
		}
	}

	public void AttackCheck(float attackDir, float attackDamage, EnemyType enemyType)
	{
		var type = _controller.CounterCheck();
		switch (type)
		{
			case AttackType.Counter:
				Debug.Log("Counter");
				Counter(false);
				break;
			case AttackType.PerfectCounter:
				Debug.Log("PerfectCounter");
				if (enemyType == EnemyType.Small)
				{
					Counter(false);
				}
				else if (enemyType == EnemyType.Middle)
				{
					Counter(true);
				}
				break;
			case AttackType.False:
				Debug.Log("False");
				Damaged(attackDir, attackDamage);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Counter(bool isPerfect)
	{
		if (isPerfect)
		{
			var hits = Physics2D.OverlapCircleAll(transform.position, perfectAttackRadius, attackLayer);
			foreach (var hit in hits)
			{
				var damageable = hit.GetComponent<IDamageable>();
				if(damageable != null) damageable.Damaged(perfectCounterDamage);
			}
			_controller.Dash(perfectAttackRadius);
		}
		else
		{
			var hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, attackLayer);
			if (hits.Length > 0)
			{
				CameraManager.Instance.SoftImpulse.GenerateImpulse();
			}
			foreach (var hit in hits)
			{
				var damageable = hit.GetComponent<IDamageable>();
				if(damageable != null) damageable.Damaged(counterDamage);
			}
		}
		
	}

	private void Damaged(float attackDir,float attackDamage)
	{
		_controller.Bash(attackDir * attackDamage);
		health -= attackDamage;
	}
}
