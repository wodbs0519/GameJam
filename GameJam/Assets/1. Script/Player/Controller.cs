﻿using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Footstep footstep;

    public CameraDepthControll depthControll;
    public float screenDurationTime;
    public float timeStopTime;

    public RawImage Capture1;

    [Header("Move")] public float maxSpeed;
    public float accelerationTime;

    [Header("counter")] public float readyTime;
    public float perfectTime;
    public float maxcounterTime;
    public float delayTime;

    public LayerMask CollisionMask;

    public ColliderBounds colliderBounds;
    public float SkinWidth;

    public float bashAddition;

    public Transform Effects;
    public Animator Cross;
    public Animator Trail;

    public ParticleSystem playerSakura;

    public float gravity;

    private float _counterTime;
    private bool _iscounter;
    private bool _isDash;

    private Animator _animator;
    private SpriteRenderer _renderer;
    private Collider2D _collider;
    private float _timeCount;
    private float _lastDir;
    private float velocityXSmoothing = 0;
    private Vector2 _velocity;
    private Coroutine CounterCoroutine;
    private float _colliderHeight;
    public float startSlideValue;
    private float _slideValue;
    public float decayValue;

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _iscounter = false;
        _counterTime = 0;
        _colliderHeight = transform.position.y - _collider.bounds.min.y;
    }

    public void Move(float moveDir)
    {
        colliderBounds = new ColliderBounds(_collider.bounds);
        Falling();

        if (_iscounter || _isDash)
        {
            footstep.StopFootstep();
            if (_isDash)
            {
                _slideValue -= Time.deltaTime * decayValue;
                _slideValue = Mathf.Clamp(_slideValue, 0, 100);
                float dir = 1;
                if (_renderer.flipX)
                {
                    dir = -1;
                }
                transform.Translate(new Vector2(_slideValue * 3 * dir * Time.deltaTime,0));
            }
            return;
        }

        var targetVelocity = CalculateVelocity(moveDir);
        _animator.SetBool("Run", true);
        if (moveDir > 0)
        {
            footstep.StartFootstep();
            _renderer.flipX = false;
            Effects.localScale = new Vector3(1, 1, 1);
        }
        else if (moveDir < 0)
        {
            footstep.StartFootstep();
            Effects.localScale = new Vector3(-1, 1, 1);
            _renderer.flipX = true;
        }
        else
        {
            footstep.StopFootstep();
            _animator.SetBool("Run", false);
            _animator.SetBool("Idle", true);
        }

        _velocity = new Vector2(targetVelocity, 0);
        if (!CheckObstacle())
        {
            transform.Translate(new Vector2(_velocity.x, 0) * Time.deltaTime);
        }
    }

    private float CalculateVelocity(float h)
    {
        float targetVelocityX = h * maxSpeed;
        float n_velocity = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTime);
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
        transform.position = targetPos;
        playerSakura.Play();
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
        _isDash = true;
        _slideValue = startSlideValue;
        Debug.Log("dash true");
    }

    public void Bash(float bashValue)
    {
        _velocity.x += bashValue * bashAddition;
        Debug.Log("Bahsed Velocity" + _velocity.x);
        StopCheck();
    }

    #region Attack

    public void StartCounter()
    {
        if (_iscounter) return;
        CounterCoroutine = StartCoroutine(ExecuteCounter());
    }

    IEnumerator ExecuteCounter()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.feetSword);
        _iscounter = true;
        _counterTime = 0;
        Cross.Play("Cross");
        _animator.Play("Ready");
        while (_counterTime < maxcounterTime)
        {
            _counterTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(delayTime);
        _animator.Play("Idle");
        _iscounter = false;
        _counterTime = 0;
    }

    public AttackType CounterCheck(bool isLast)
    {
        AttackType type;
        if (_counterTime == 0 || _counterTime >= maxcounterTime)
        {
            type = AttackType.False;
            return type;
        }
//        else if (_counterTime < perfectTime)
//        {
//            type =  AttackType.PerfectCounter;
//        }
        
        type = AttackType.Counter;
        
        StopCheck();
        StartCoroutine(DepthScreen(type, isLast));

        if (!isLast)
        {
            var tempVal = Vector3.one;
            Trail.transform.localScale = tempVal;
            int a = Random.Range(0, 2);
            if (a == 0)
            {
                Trail.Play("Trail");
            }
            else
            {
                Trail.Play("Trail2");
            }
        }

        return type;
    }

    IEnumerator DepthScreen(AttackType attackType, bool isLast)
    {
        _iscounter = true;
        
        AudioManager.instance.PlaySound(AudioManager.instance.swingSword);
        AudioManager.instance.PlaySound(AudioManager.instance.playerAttack);

        if (isLast)
        {
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        }

        depthControll.enabled = true;
        _animator.Play("Attack");
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(timeStopTime);
        Time.timeScale = 1;

        yield return new WaitForSeconds(screenDurationTime);

        _animator.Play("Idle");
        _isDash = false;    
        _iscounter = false;
        Debug.Log("dash false");
        depthControll.enabled = false;
    }

    public void StopCheck()
    {
        if (CounterCoroutine != null)
            StopCoroutine(CounterCoroutine);
        _counterTime = 0;
        _iscounter = false;
        _animator.Play("Idle");
    }

    #endregion


    #region CheckCollider

    private void Falling()
    {
        float dist = _colliderHeight + gravity * Time.deltaTime;
        var hit = Physics2D.Raycast(transform.position, Vector2.down, dist, CollisionMask);
        if (hit.collider == null)
        {
            transform.Translate(Time.deltaTime * gravity * Vector2.down);
        }
        else
        {
            float hDist = transform.position.y - hit.point.y;
            transform.position = (Vector2) transform.position + new Vector2(0, _colliderHeight - hDist);
        }
    }

    private bool CheckObstacle()
    {
        Vector2 raycasyOrigin;
        if (_velocity.x > 0)
        {
            raycasyOrigin = colliderBounds.BottomRight;
        }
        else
        {
            raycasyOrigin = colliderBounds.BottomLeft;
        }

        raycasyOrigin.y += SkinWidth;
        raycasyOrigin.x -= SkinWidth * Mathf.Sign(_velocity.x);

        var hit = Physics2D.Raycast(raycasyOrigin, Vector2.right * Mathf.Sign(_velocity.x), 0.2f, CollisionMask);

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    #endregion
}

public struct ColliderBounds
{
    public Vector2 BottomLeft;
    public Vector2 BottomRight;
    public Vector2 BottomCenter;

    public ColliderBounds(Bounds bounds)
    {
        BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        BottomCenter = new Vector2((BottomLeft.x + BottomRight.x) * 0.5f, BottomLeft.y);
    }
}


public enum AttackType
{
    Counter,
    PerfectCounter,
    False
}