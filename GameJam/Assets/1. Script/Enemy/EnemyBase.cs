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
    public float gravity = 1;

    public ColliderBounds colliderBounds;

    public LayerMask CollisionMask;

    public float stunTime;

    private float _timeCount;

    public float rippleDelay;
    public ParticleSystem rippleParticle;

    public EnemyType Type;
    public float SkinWidth;
    public float accelerationTime;

    public int Health
    {
        get { return _health; }
        set
        {
            var oldValue = _health;
            _health = value;
            if (_health <= 0)
            {
                Dead();
            }
            else if(oldValue > _health)
            {
                var targetVector = new Vector3(transform.position.x, transform.position.y, -5f);
                var sakura = Instantiate(Sakura, targetVector, Quaternion.Euler(0, 90 + Mathf.Sign(_targetDir.x) * 90, 0),
                    null);
                sakura.Play();
                Destroy(sakura.gameObject, 2);
                Bashed();
            }
        }
    }

    public int startHealth;

    private int _health = 3;
    private bool _isAttack;
    private Vector2 _targetDir;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private float _velocity;
    private float _colliderHeight;
    private float velocityXSmoothing = 0;
    private bool _isDead = false;

    public Transform Target;
    public Action deadCallback;
    public ParticleSystem Sakura;

    private Coroutine _coroutine;

    private void Start()
    {
        Target = Player.instance.transform;
        Health = startHealth;
        _isAttack = false;
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _colliderHeight = transform.position.y - _collider.bounds.min.y;
        StartCoroutine(MakeRipple());
    }

    IEnumerator MakeRipple()
    {
        while (true)
        {
            rippleParticle.transform.position = new Vector2(transform.position.x, _collider.bounds.min.y);
            rippleParticle.Play();
            yield return new WaitForSeconds(rippleDelay);
        }
    }


    private void Update()
    {
        if (_isDead) return;
        colliderBounds = new ColliderBounds(_collider.bounds);
        Falling();

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
            ReadyAttack();
        }

        if (CheckObstacle()) return;

        if (_timeCount > 0)
        {
            _timeCount -= Time.deltaTime;
            Bashed();
        }

        if (_isAttack)
        {
            _velocity = 0;
        }

        transform.Translate(new Vector2(_velocity, 0) * Time.deltaTime);
    }

    private void Walk()
    {
        float targetVelocityX = walkSpeed * Mathf.Sign(_targetDir.x);
        _velocity = Mathf.SmoothDamp(_velocity, targetVelocityX, ref velocityXSmoothing, accelerationTime);
    }

    private void Run()
    {
        float targetVelocityX = runSpeed * Mathf.Sign(_targetDir.x);
        _velocity = Mathf.SmoothDamp(_velocity, targetVelocityX, ref velocityXSmoothing, accelerationTime);
    }


    public void ReadyAttack()
    {
        if (_isAttack || _timeCount > 0) return;
        _isAttack = true;
        _animator.SetTrigger("Attack");
    }

    public void StartAttack()
    {
       _coroutine = StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Player.instance.AttackCheck(Mathf.Sign(_targetDir.x), attackDamage, Type);
        yield return new WaitForSeconds(attackDelay);
        _isAttack = false;
    }


    private void Dead()
    {
        _isDead = true;
        Debug.Log("I'm Dead");
        _animator.Play("Dead");
        StartCoroutine(WaitDead());
    }

    IEnumerator WaitDead()
    {
        yield return null;
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
        if (deadCallback != null) deadCallback();
        DestroyImmediate(gameObject);
    }


    public void Damaged(int damage)
    {
        if(_coroutine!=null)
            StopCoroutine(_coroutine);
        
        _isAttack = false;
        _timeCount = stunTime;
        _animator.SetTrigger("Paused");
        Health -= damage;
    }

    public void Bashed()
    {
        _velocity = Mathf.Sign(_targetDir.x) * -bashAddition;
    }

    private void Falling()
    {
        float dist = _colliderHeight + gravity * Time.deltaTime;
        var hits = Physics2D.RaycastAll(transform.position, Vector2.down, dist, CollisionMask);

        if (hits.Length <= 1)
        {
            transform.Translate(Time.deltaTime * gravity * Vector2.down);
        }
        else
        {
            float hDist = transform.position.y - hits[1].point.y;
            transform.position = (Vector2) transform.position + new Vector2(0, _colliderHeight - hDist);
        }
    }

    private bool CheckObstacle()
    {
        Vector2 raycasyOrigin;
        if (_velocity > 0)
        {
            raycasyOrigin = colliderBounds.BottomRight;
        }
        else
        {
            raycasyOrigin = colliderBounds.BottomLeft;
        }

        raycasyOrigin.y += SkinWidth;
        raycasyOrigin.x -= SkinWidth * Mathf.Sign(_velocity);

        var hits = Physics2D.RaycastAll(raycasyOrigin, Vector2.right * Mathf.Sign(_velocity), 0.4f, CollisionMask);
        ;

        if (hits.Length <= 1)
        {
            return false;
        }

        return true;
    }
}

public enum EnemyType
{
    Small,
    Middle,
    Large
}