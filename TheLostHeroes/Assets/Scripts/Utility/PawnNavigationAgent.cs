using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public static class PawnNavigationAgent
{
    public static void Initialize(Pawn pawn)
    {
        var agent = pawn.self.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public static void UpdateTarget(Pawn pawn)
    {
        if (pawn.netFields.taskID == -1)
        {
            SetTargetPosition(pawn, pawn.self.transform.position);
            return;
        }

        // TODO: выбирать позицию цели рядом, а не точно в цели?
        SetTargetPosition(pawn, NetEntitySyncronizer.MustGetComponent<Task>(pawn.netFields.taskID).instance.transform.position);
    }

    private static void SetTargetPosition(Pawn pawn, Vector3 position)
    {
        var agent = pawn.self.GetComponent<NavMeshAgent>();
        agent.SetDestination(position);
    }
}
