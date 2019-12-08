using System;
using System.Collections;
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
    public float dashRadius;
    public LayerMask attackLayer;
    public int counterDamage;
    public float attackedTime;

    private float _health;
    private Controller _controller;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        instance = this;
        _controller = GetComponent<Controller>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        health = StartHealth;
    }

    // Update is called once per frame
    void Update()
    {
        var moveDir = Input.GetAxisRaw("Horizontal");
        _controller.Move(moveDir);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _controller.StartCounter();
        }
    }

    public bool AttackCheck(float attackDir, float attackDamage, bool isLast)
    {
        var type = _controller.CounterCheck(isLast);
        switch (type)
        {
            case AttackType.Counter:
                Counter(isLast);
                break;
            case AttackType.PerfectCounter:
                Counter(isLast);
                break;
            case AttackType.False:
                Debug.Log("False");
                Damaged(attackDir, attackDamage);
                return true;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    private void Counter(bool isLast)
    {
//		if (isPerfect)
//		{
//			var hits = Physics2D.OverlapCircleAll(transform.position, perfectAttackRadius, attackLayer);
//			foreach (var hit in hits)
//			{
//				var damageable = hit.GetComponent<IDamageable>();
//				if(damageable != null) damageable.Damaged(perfectCounterDamage);
//			}
//			_controller.Dash(perfectAttackRadius);
//		}
//		else
//		{
//			var hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, attackLayer);
//			if (hits.Length > 0)
//			{
//				CameraManager.Instance.SoftImpulse.GenerateImpulse();
//			}
//			foreach (var hit in hits)
//			{
//				var damageable = hit.GetComponent<IDamageable>();
//				if(damageable != null) damageable.Damaged(counterDamage);
//			}
//		}

        var hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, attackLayer);
        if (hits.Length > 0)
        {
            CameraManager.Instance.SoftImpulse.GenerateImpulse();
        }

        foreach (var hit in hits)
        {
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if(!isLast)
                    damageable.Damaged(counterDamage);
                else
                {
                    damageable.Dead();
                }
            }
        }

        if (isLast)
        {
            _controller.Dash(dashRadius);
        }
    }

    private void Damaged(float attackDir, float attackDamage)
    {
        _controller.Bash(attackDir * attackDamage);
        health -= attackDamage;
        StartCoroutine(AlphaChange());
    }

    IEnumerator AlphaChange()
    {
        float timeCount = 0;
        Color c = Color.white;
        while (true)
        {
            timeCount += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, timeCount / attackedTime);
            _renderer.color = c;
            if (timeCount > attackedTime) break;
            yield return null;
        }

        timeCount = 0;
        while (true)
        {
            timeCount += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, timeCount / attackedTime);
            _renderer.color = c;
            if (timeCount > attackedTime) break;
            yield return null;
        }
        timeCount = 0;
        while (true)
        {
            timeCount += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, timeCount / attackedTime);
            _renderer.color = c;
            if (timeCount > attackedTime) break;
            yield return null;
        }

        timeCount = 0;
        while (true)
        {
            timeCount += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, timeCount / attackedTime);
            _renderer.color = c;
            if (timeCount > attackedTime) break;
            yield return null;
        }
    }
}