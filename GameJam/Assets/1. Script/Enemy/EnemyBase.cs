using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{

    public string[] Attacks;
    
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
            _health = value;
            if (_health <= 0)
            {
                _isLast = true;
            }
            else
            {
                _isLast = false;
            }
        }
    }

    public int startHealth;

    private int _health = 0;
    private bool _isAttack;
    private Vector2 _targetDir;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private float _velocity;
    private float _colliderHeight;
    private float velocityXSmoothing = 0;
    private bool _isDead = false;
    private bool _isLast;
    
    public Transform Target;
    public Action deadCallback;
    public ParticleSystem Sakura;

    private Coroutine _coroutine;

    private void Start()
    {
        Target = Player.instance.transform;
        _isAttack = false;
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _colliderHeight = transform.position.y - _collider.bounds.min.y;
        Health = startHealth;
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

        if (_timeCount > 0)
        {
            _timeCount -= Time.deltaTime;
        }
        
        if (CheckObstacle()) return;

        

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
        _coroutine = StartCoroutine(AttackCycle());
    }

    IEnumerator AttackCycle(float dealyTime = 0)
    {
        yield return new WaitForSeconds(dealyTime);
        Debug.Log("Attack Cycle Start");
        Health = startHealth;
        int index = 0;
        while (index < Attacks.Length)
        {
            _animator.Play(Attacks[index], 0, 0);
            Debug.Log("Attack " + index);
            yield return null;
            yield return new WaitWhile(() =>
            {
                var currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
                return (currentInfo.IsName(Attacks[index]) && currentInfo.normalizedTime < 1);
            });
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Bashed"))
            {
                yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
            }
            index++;
            Debug.Log("Index Pluse");
        }
        _animator.Play("Run");
        yield return new WaitForSeconds(attackDelay);
        _isAttack = false;
        
    }
    

    public void StartAttack()
    {
        if (Player.instance.AttackCheck(Mathf.Sign(_targetDir.x), attackDamage, _isLast))
        {
            StopCoroutine(_coroutine);
            _animator.Play("Run");
            _isAttack = false;
            _timeCount = 0.5f;
        }
        
    }

    public void Dead()
    {
        _isDead = true;
        StopCoroutine(_coroutine);
        _animator.Play("Dead");
        StartCoroutine(WaitDead());
        AudioManager.instance.PlaySound(AudioManager.instance.enemyDead);
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
        _timeCount = stunTime;
        Health -= damage;
        if (Health >= 0)
        {
            Bashed();
        }
    }

    public void Bashed()
    {
        _animator.Play("Bashed");
        var targetVector = new Vector3(transform.position.x, transform.position.y, -5f);
        var sakura = Instantiate(Sakura, targetVector, Quaternion.Euler(0, 90 + Mathf.Sign(_targetDir.x) * 90, 0),
            null);
        sakura.Play();
        Destroy(sakura.gameObject, 4);
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