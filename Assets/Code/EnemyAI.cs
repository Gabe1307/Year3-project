using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour

{

    //Stored Data 

    float distanceToTarget = Mathf.Infinity;


    [SerializeField] float chaseRange = 5f;

    [SerializeField] float turnSpeed = 5f;

    //[SerializeField] float enemyHP = 100f;


    //attack player
    [SerializeField] float damage = 20f;
    [SerializeField] PlayerHealth target;





    //cashed references 

    [SerializeField] Transform playerTarget;

    NavMeshAgent nMA;

    Animator anim;

    //states 

    bool isAggro = false;



    void Start()

    {

        nMA = GetComponent<NavMeshAgent>();
       // anim = GetComponent<Animator>();

    }

    // Update is called once per frame 

    void Update()

    {
        distanceToTarget = Vector3.Distance(playerTarget.position, transform.position);

        if (isAggro)
        {
            EngageTarget();
            if(distanceToTarget > chaseRange)
            {
                isAggro = false;
            }
        }

        else if (distanceToTarget <= chaseRange)

        {
            isAggro = true;
        }

       
    }

    private void EngageTarget()
    {
        FaceTarget();

        if (distanceToTarget >= nMA.stoppingDistance)
        {
            ChasePlayer();
        }

        else if (distanceToTarget <= nMA.stoppingDistance)
        {
            AttackPlayer();

        }

    }

    private void ChasePlayer()
    {
        print("Chasing");
        nMA.SetDestination(playerTarget.position);
    }

    private void AttackPlayer()
    {
        print("Attacking");

        target.TakeDamage(damage);
        Application.LoadLevel(Application.loadedLevel);

    }
    public void OnDamageTaken()
    {
        isAggro = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    void FaceTarget()
    {
        Vector3 direction = (playerTarget.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

}