using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMesh : MonoBehaviour
{
    public NavMeshAgent enemy;
    public Transform player;
    
    void Start()
    {
        
    }

 
    void Update()
    {
        enemy.SetDestination(player.position);
    }
}
