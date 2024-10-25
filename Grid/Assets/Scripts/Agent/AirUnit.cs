//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(MoveComponent))]
//public class AirUnit : MonoBehaviour
//{
//    [SerializeField] Transform _endTarget;
//    [SerializeField] AirPathfinder _pathfinder;
//    [SerializeField] float _moveSpeed;
//    MoveComponent _moveComponent;
    
//    // Start is called before the first frame update
//    void Start()
//    {
//        _moveComponent = GetComponent<MoveComponent>();
//        _moveComponent.Initialize(_pathfinder.FindPath, true, _moveSpeed);
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        _moveComponent.Move(_endTarget.position);
//    }
//}
