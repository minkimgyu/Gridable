using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

namespace MultiThreadPathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        [SerializeField] int maxThreadCount = 20;

        Queue<PathResult> results = new Queue<PathResult>();

        static PathRequestManager instance;
        public static PathRequestManager Instance { get { return instance; } }

        Pathfinder pathfinder;

        public void Initialize(GridComponent grid)
        {
            pathfinder = new Pathfinder(grid);
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
        }

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                for (int i = 0; i < itemsInQueue; i++)
                {
                    PathResult result = results.Dequeue();
                    result.callback(result.path, result.success);
                }
            }
        }

        public void RequestPath(PathRequest request)
        {
            ThreadPool.QueueUserWorkItem((obj) => { pathfinder.FindPath(request, FinishedProcessingPath); });
        }

        public void FinishedProcessingPath(PathResult result)
        {
            lock (results)
            {
                results.Enqueue(result);
            }
        }
    }

    public struct PathResult
    {
        public List<Vector3> path;
        public bool success;
        public Action<List<Vector3>, bool> callback;

        public PathResult(List<Vector3> path, bool success, Action<List<Vector3>, bool> callback)
        {
            this.path = path;
            this.success = success;
            this.callback = callback;
        }

    }

    public struct PathRequest
    {
        public Vector3 start;
        public Vector3 end;
        public int safeRange;
        public Action<List<Vector3>, bool> callback;

        public PathRequest(Vector3 start, Vector3 end, int safeRange, Action<List<Vector3>, bool> callback)
        {
            this.start = start;
            this.end = end;
            this.safeRange = safeRange;
            this.callback = callback;
        }
    }
}