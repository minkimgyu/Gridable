using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace FlowFieldPathfinding
{
    public class EndPoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Agent agent = other.GetComponent<Agent>();
            if (agent == null) return;

            agent.FinishPath();
        }
    }

}