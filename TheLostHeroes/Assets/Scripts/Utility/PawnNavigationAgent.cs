using UnityEngine;
using UnityEngine.AI;

public static class PawnNavigationAgent
{
    public static void Initialize(Pawn pawn) {
        pawn.self.GetComponent<NavMeshAgent>().updateRotation = false;
        pawn.self.GetComponent<NavMeshAgent>().updateUpAxis = false;
    }

    public static void UpdateTarget(Pawn pawn)
    {
        // TODO: выбирать позицию цели рядом, а не точно в цели?
        Vector3 TargetPos = NetEntitySyncronizer.MustGetComponent<Task>(pawn.netFields.taskID).instance.transform.position;
        pawn.self.GetComponent<NavMeshAgent>().SetDestination(TargetPos);
    }
}
