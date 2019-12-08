using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour, IDamageable
{
    private SpriteRenderer _renderer;
    private Animator _animator;
    
    private Vector2 targetPos;
    public float speed;
    public ParticleSystem ripple;

    public Action MakeArrow;
    
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();

        StartCoroutine(ChaseTarget());
    }

    
    Quaternion LookAt2D(Vector2 forward) {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.AttackCheck(-1, 1, false);
            Dead();
        }
    }

    public void Damaged(int damage)
    {
        Dead();
    }

    public void Dead()
    {
        _animator.Play("Dead");
        if(MakeArrow != null)
            MakeArrow();
        ripple.Stop();
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, .7f);
    }

    IEnumerator ChaseTarget()
    {
        while (true)
        {
            targetPos = Player.instance.transform.position;
            // Compute the next position -- straight flight
            Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            // Rotate to face the next position, and then move there
            transform.rotation = LookAt2D(nextPos - transform.position);
            transform.position = nextPos;
            yield return null;
        }
    }
}