﻿using UnityEngine;
using System.Collections;

public class NewFlyer : MonoBehaviour
{
    private enum FlyerState
    {
        Patrol,
        Threaten,
        Attack,
    }

    private Patrol patrol;

    private FlyerState curState;

    private Transform player;

    [SerializeField]
    private float DistanceToThreaten = 8f;

    [SerializeField]
    private float AttackSpeed = 15f;

    [SerializeField]
    private float TimeToThreaten = 10;

    [SerializeField]
    private float TimeToAttack = 3;


    [SerializeField]
    private PlayerAttack diveAttack;

    [SerializeField]
    private EnemyAttackArea attackArea;

    private bool isThreatening = false;
    private bool isAttacking = false;

    // Use this for initialization
    void Start()
    {
        patrol = GetComponent<Patrol>();
        curState = FlyerState.Patrol;


        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case FlyerState.Patrol:
                if (Vector3.Distance(transform.position, player.position) <= DistanceToThreaten)
                {
                    curState = FlyerState.Threaten;
                }
                else
                    patrol.Patrolling();
                break;
            case FlyerState.Threaten:
                if (!isThreatening)
                {
                    StopCoroutine(Threat());
                    StartCoroutine(Threat());
                }
                break;
            case FlyerState.Attack:
                if (!isAttacking)
                {
                    StopCoroutine(Attack());
                    StartCoroutine(Attack());
                }
                break;
        }
    }


    IEnumerator Threat()
    {
        isThreatening = true;
        float timeTaken = 0;

        while (timeTaken < TimeToThreaten)
        {
            timeTaken += Time.deltaTime;

            Vector3 toLookAt = player.position - transform.position;
            toLookAt.y = 0;

            Quaternion lr = Quaternion.LookRotation(toLookAt);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lr, patrol.RotateSpeed * Time.deltaTime);

            transform.position += Vector3.up * Mathf.Lerp(0, (player.position.y + 4.25f) - transform.position.y, Time.deltaTime);
            yield return null;
        }


        isThreatening = false;
        curState = FlyerState.Attack;
        yield return null;
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        float timeTaken = 0;
        Quaternion init = new Quaternion(transform.rotation.x, transform.rotation.y,transform.rotation.z,transform.rotation.w);
        attackArea.ActivateAttack(diveAttack);
        transform.LookAt(player);

        while (timeTaken < TimeToAttack)
        {
            timeTaken += Time.deltaTime;

            transform.position += transform.forward * AttackSpeed * Time.deltaTime;

            yield return null;
        }

        attackArea.EndAttack();

        timeTaken = 0;

        while (timeTaken < TimeToAttack * 2)
        {
            timeTaken += Time.deltaTime;

            transform.position -= transform.forward * AttackSpeed  * .5f * Time.deltaTime;

            yield return null;
        }

        transform.rotation = init;

        isAttacking = false;
        curState = FlyerState.Patrol;
        yield return null;
    }
}