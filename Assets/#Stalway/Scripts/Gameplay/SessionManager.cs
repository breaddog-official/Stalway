using System;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class SessionManager : MonoBehaviour
    {
        [SerializeField] private Session defaultSession;

        private PoolManager poolManager;
        private Session session;

        public void Initialize()
        {

        }

        public void InitializeSession(Session session = null)
        {
            session ??= defaultSession;
            session.Initialize();

            this.session = session;
        }
    }

    [Serializable, BurstCompile]
    public class Session
    {
        public void Initialize()
        {

        }
    }
}
