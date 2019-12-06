using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{

    [Header("Move")]
    public float maxSpeed;
    public float accelerationTime;

    [Header("counter")] 
    public float readyTime;
    public float perfectTime;
    public float maxcounterTime;
    public float delayTime;
    
    public LayerMask CollisionMask;

    public float bashAddition;
    
    private float _counterTime;
    private bool _iscounter;
    
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private float _timeCount;
    private float _lastDir;
    private float velocityXSmoothing = 0;
    private Coroutine CounterCoroutine;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _iscounter = false;
        _counterTime = 0;
    }

    public void Move(float moveDir)
    {
        var targetVelocity = CalculateVelocity(moveDir);
        _animator.SetBool("Run", true);
        if (moveDir > 0)
        {
            _renderer.flipX = false;
        }
        else if(moveDir < 0)
        {
            _renderer.flipX = true;
        }
        else
        {
            _animator.SetBool("Run", false);
            _animator.SetBool("Idle", true);
        }
        _rigidbody.velocity = new Vector2(targetVelocity, _rigidbody.velocity.y);
    }

    public void StartCounter()
    {
        if (_counterTime > 0) return;
        CounterCoroutine = StartCoroutine(ExecuteCounter());
    }

    IEnumerator ExecuteCounter()
    {
        yield return new WaitForSeconds(readyTime);
        
        _iscounter = true;
        while (_counterTime < maxcounterTime)
        {
            _counterTime += Time.deltaTime;
            yield return null;
        }

        _iscounter = false;
        
        yield return new WaitForSeconds(delayTime);
        _counterTime = 0;
    }

    public AttackType CounterCheck()
    {
        AttackType type;
        if (!_iscounter)
        {
            type =  AttackType.False;
        }
        else if (_counterTime < perfectTime)
        {
            type =  AttackType.PerfectCounter;
        }
        else
        {
            type = AttackType.Counter;
        }
        StopCheck();
        return type;
    }

    public void StopCheck()
    {
        if(CounterCoroutine != null)
            StopCoroutine(CounterCoroutine);
        _counterTime = 0;
        _iscounter = false;
    }
    


    private float CalculateVelocity(float h)
    {
        float targetVelocityX = h * maxSpeed;
        float n_velocity = Mathf.SmoothDamp(_rigidbody.velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
        return n_velocity;
    }

    public void Dash(float dashRange)
    {
        float dir = 1;
        if (_renderer.flipX)
        {
            dir = -1;
        }
        var hit = Physics2D.Raycast(transform.position,Vector2.right * dir, dashRange, CollisionMask);
        Vector2 targetPos = (Vector2)transform.position + new Vector2(dashRange * dir, 0);
        if (hit.collider != null) targetPos = hit.point;
        transform.position = targetPos;
    }

    public void Bash(float bashValue)
    {
        _rigidbody.AddForce(Vector2.right * bashValue * bashAddition);
        StopCheck();
    }
    
    
}

public enum AttackType
{
    Counter,
    PerfectCounter,
    False
}
