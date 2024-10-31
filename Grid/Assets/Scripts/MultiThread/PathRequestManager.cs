//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace MultiThreadPathfinding
//{
//    public class PathRequestManager : MonoBehaviour
//    {
//        [SerializeField] int maxThreadCount = 20;

//        //Queue<PathResult> results = new Queue<PathResult>();

//        static PathRequestManager instance;
//        public static PathRequestManager Instance { get { return instance; } }

//        Pathfinder pathfinder;

//        public void Initialize(GridComponent grid)
//        {
//            pathfinder = new Pathfinder(grid);
//            //ThreadPool.SetMinThreads(1, 1);
//            //ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
//        }

//        void Awake()
//        {
//            instance = this;
//        }

//        //void Update()
//        //{
//        //    if (results.Count > 0)
//        //    {
//        //        int itemsInQueue = results.Count;
//        //        for (int i = 0; i < itemsInQueue; i++)
//        //        {
//        //            PathResult result = results.Dequeue();
//        //            result.callback(result.path, result.success);
//        //        }
//        //    }
//        //}

//        //public async Task<PathResult> RequestPath(PathRequest request)
//        //{
//        //    return await Task<PathResult>.Run(() => { return pathfinder.FindPath(request); });
//        //    //ThreadPool.QueueUserWorkItem((obj) => { pathfinder.FindPath(request, FinishedProcessingPath); });
//        //}

//        //public void FinishedProcessingPath(PathResult result)
//        //{
//        //    lock (results)
//        //    {
//        //        results.Enqueue(result);
//        //    }
//        //}
//    }

    
//}