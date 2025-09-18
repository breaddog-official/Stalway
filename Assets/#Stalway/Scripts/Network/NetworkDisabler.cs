using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breaddog.Gameplay
{
    public class NetworkDisabler : NetworkBehaviour
    {
        [Range(0, 20)]
        public int UpdateFrames = 5;
        public Dictionary<Behaviour, EnableConfig> Components;

        int currentFrames;


        private void OnEnable()
        {
            UpdateStates();
        }
        protected virtual void Update()
        {
            currentFrames++;
            if (currentFrames > UpdateFrames)
            {
                currentFrames = 0;
                UpdateStates();
            }
        }

        public void UpdateStates()
        {
            foreach (var pair in Components)
            {
                bool state = (pair.Value.connectionMode != ConnectionMode.Client || NetworkClient.active) &&
                             (pair.Value.connectionMode != ConnectionMode.Server || NetworkServer.active) &&
                             (pair.Value.connectionMode != ConnectionMode.Host || (NetworkClient.active && NetworkServer.active)) &&
                             (pair.Value.ownedMode != OwnedMode.Owned || isOwned) &&
                             (pair.Value.ownedMode != OwnedMode.NotOwned || !isOwned);

                pair.Key.enabled = state;
            }
        }

        [Serializable]
        public struct EnableConfig
        {
            public ConnectionMode connectionMode;
            public OwnedMode ownedMode;
        }

        public enum ConnectionMode
        {
            NotMatter,
            Client,
            Server,
            Host,
        }

        public enum OwnedMode
        {
            NotMatter,
            Owned,
            NotOwned
        }
    }
}
