using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpriteUpdater : MonoBehaviour
{
    private NavMeshAgent agent;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        agent = transform.parent.GetComponent<NavMeshAgent>();   
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sprite.flipX = Vector3.Angle(new Vector3(1, 0), agent.steeringTarget - transform.position) > 90;
    }
}
