using UnityEngine;
using System;
using System.Collections.Generic;
#if (NET_4_6 || NETFX_CORE)
using System.Threading.Tasks;
#else
using System.Threading;
#endif

namespace TriLib
{
    /// <summary>
    /// A system for dispatching code to execute on the main thread.
    /// </summary>
    public class Dispatcher : MonoBehaviour
    {
        private static Dispatcher _instance;
        private static bool _instanceExists;
        private static readonly object LockObject = new object();
        private static readonly Queue<Action> Actions = new Queue<Action>();

        /// <summary>
        /// Checks if there is any instance.
        /// </summary>
        public static void CheckInstance() {
            if (!_instanceExists)
            {
                var gameObject = new GameObject("Dispatcher");
                gameObject.AddComponent<Dispatcher>();
                _instanceExists = true;
            }
        }

        /// <summary>
        /// Queues an action to be invoked on the main game thread.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        public static void InvokeAsync(Action action)
        {
            if (!_instanceExists)
            {
                Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
                return;
            }
            lock (LockObject)
            {
                Actions.Enqueue(action);
            }
        }

        void Awake()
        {
            if (_instance)
            {
                DestroyImmediate(this);
            }
            else
            {
                _instance = this;
                _instanceExists = true;
            }
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _instanceExists = false;
            }
        }

        void Update()
        {
            lock (LockObject)
            {
                while (Actions.Count > 0)
                {
                    Actions.Dequeue()();
                }
            }
        }
    }
}
