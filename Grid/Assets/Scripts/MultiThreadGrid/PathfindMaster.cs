using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;

namespace GridMaster
{
    //This class controls the threads
    public class PathfindMaster : MonoBehaviour
    {
        //Singleton
        private static PathfindMaster instance;
        void Awake()
        {
            instance = this;
        }
        public static PathfindMaster GetInstance()
        {
            return instance;
        }

        //The maximum simultaneous threads we allow to open
        public int MaxJobs = 3;

        //Delegates are a variable that points to a function
        public delegate void PathfindingJobComplete(List<Vector3> path);

        private List<Pathfinder> currentJobs;
        private List<Pathfinder> todoJobs;

        [SerializeField] int todoCount;
        [SerializeField] int currentCount;

        void Start()
        {
            currentJobs = new List<Pathfinder>();
            todoJobs = new List<Pathfinder>();

            ThreadPool.SetMinThreads(1, 1); //최소 스레드 개수
            ThreadPool.SetMaxThreads(MaxJobs, MaxJobs); //최대 스레드 개수
        }
   
        void Update() 
        {
            /*
             * Another way to keep track of the threads we have open would have been to create 
             * a new thread for it but we can also just use Unity's main thread too since this class
             * derives from monoBehaviour
             */

            int i = 0;

            while(i < currentJobs.Count)
            {
                if(currentJobs[i].jobDone)
                {
                    currentJobs[i].NotifyComplete();
                    currentJobs.RemoveAt(i);
                    currentCount--;
                }
                else
                {
                    i++;
                }
            }

            if(todoJobs.Count > 0 && currentJobs.Count < MaxJobs) // 사실상 프레임당 하나씩만 실행된다.
            {
                Pathfinder job = todoJobs[0];
                todoJobs.RemoveAt(0);
                currentJobs.Add(job);
                todoCount--;

                //Start a new thread

                ThreadPool.QueueUserWorkItem((obj) => { job.FindPath(); });
                currentCount++;

                //Thread jobThread = new Thread(job.FindPath);
                //jobThread.Start();

                //As per the doc
                //https://msdn.microsoft.com/en-us/library/system.threading.thread(v=vs.110).aspx
                //It is not necessary to retain a reference to a Thread object once you have started the thread. 
                //The thread continues to execute until the thread procedure is complete.				
            }
        }

        //readonly object lockPathfind = new object();

        public void RequestPathfind(Node start, Node target, PathfindingJobComplete completeCallback)
        {
            //lock (lockPathfind)
            //{
                Pathfinder newJob = new Pathfinder(start, target, completeCallback);
                todoJobs.Add(newJob);
                todoCount++;
            //}
        }
    }
}
