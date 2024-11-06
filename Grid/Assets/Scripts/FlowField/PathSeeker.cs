using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowFieldPathfinding
{
    public class PathSeeker : MonoBehaviour
    {
        GridComponent _gridComponent;

        public void Initialize(GridComponent gridComponent)
        {
            _gridComponent = gridComponent;
        }

        public Vector3 ReturnDirection()
        {
            return _gridComponent.ReturnNodeDirection(transform.position);
        }
    }
}