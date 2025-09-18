using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

namespace Breaddog.Network
{
    [AddComponentMenu("Network/ Interest Management/ Prediction/Prediction Interest Management")]
    public class PredictionInterestManagement : InterestManagement
    {
        [Flags]
        public enum VisibleBehaviour
        {
            None = 0,
            Distance = 1 << 0,
            Linecast = 1 << 1,
            Prediction = 1 << 2,
        }

        public enum RaysDirection
        {
            Positive,
            Negative,
            Alternating
        }

        [Header("Interest Management")]
        [Min(1)] public uint RebuildEveryFrames = 2;
        [Min(0)] public float MaxDistance = 25f;
        public VisibleBehaviour DefaultBehaviour;
        public VisibleBehaviour SameFractionBehaviour;
        [Header("Prediction")]
        public int HorizontalRaysCount = 64;
        public float HorizontalRaysSpace = 0.25f;
        public RaysDirection HorizontalRaysDirection;
        public int VerticalRaysCount = 4;
        public float VerticalRaysSpace = 1f;
        public RaysDirection VerticalRaysDirection;
        [Space]
        public float MaxPredictionDistance = 3f;
        public LayerMask RaycastLayerMask;
        [Header("Debug")]
        public bool EnableLogging;
        public bool DrawGizmos;

        private const float GIZMOS_LENGTH = 5.0f;

        private uint currentRebuildFrame;
        private Vector3 gizmosIdentity = Vector3.zero;
        private Vector3 gizmosDirectionToSecond = Vector3.forward * GIZMOS_LENGTH;
        private Vector3 gizmosDirectionToWall = Vector3.right * GIZMOS_LENGTH;
        private readonly List<Vector3> gizmosDirections = new();

        private readonly Dictionary<NetworkIdentity, IInterestOverrider> interestOverriders = new();






        [ServerCallback]
        private void Update()
        {
            // rebuild all spawned NetworkIdentity's observers every 'rebuildEveryFrames'
            if (++currentRebuildFrame == RebuildEveryFrames)
            {
                currentRebuildFrame = 0;
                RebuildAll();
            }
        }

        [ServerCallback]
        public override void ResetState()
        {
            currentRebuildFrame = 0;
            interestOverriders.Clear();
        }


        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
        {
            // authenticated and joined world with a player?
            if (newObserver != null && newObserver.isAuthenticated && newObserver.identity != null)
            {
                return IsVisible(newObserver.identity, identity);
            }
            else
            {
                return false;
            }
        }

        public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
        {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (OnCheckObserver(identity, conn))
                {
                    newObservers.Add(conn);
                }
            }
        }


        public override void OnSpawned(NetworkIdentity identity)
        {
            var overrider = identity.GetComponent<IInterestOverrider>() ?? identity.GetComponentInChildren<IInterestOverrider>();

            if (overrider != null)
                interestOverriders.Add(identity, overrider);
        }

        public override void OnDestroyed(NetworkIdentity identity)
        {
            interestOverriders.Remove(identity);
        }



        public bool IsVisible(NetworkIdentity identity, NetworkIdentity observer)
        {
            Transform identityTransform = interestOverriders.GetValueOrDefault(identity)?.InterestTransform ?? identity.transform;
            Transform observerTranform = interestOverriders.GetValueOrDefault(observer)?.InterestTransform ?? observer.transform;

            VisibleBehaviour behaviour = DefaultBehaviour;

            if (behaviour != SameFractionBehaviour && IsSameFraction(identity, observer))
                behaviour = SameFractionBehaviour;

            // Check distance
            if (behaviour.HasFlag(VisibleBehaviour.Distance) && !VisibleByDistance(identityTransform, observerTranform))
                return false;

            // Check linecast
            if (behaviour.HasFlag(VisibleBehaviour.Linecast))
            {
                if (VisibleByLinecast(identityTransform, observerTranform))
                    return true;
                // If not visible by linecast and we will not predict, return false
                else if (!behaviour.HasFlag(VisibleBehaviour.Prediction))
                    return false;
            }

            // Check prediction
            if (behaviour.HasFlag(VisibleBehaviour.Prediction) && !VisibleByPrediction(identityTransform, observerTranform))
                return false;

            return true;
        }


        #region Checks

        public bool IsSameFraction(NetworkIdentity identity, NetworkIdentity observer)
        {
            /*if (dataFractions.TryGetValue(observer, out var identityData) &&
                dataFractions.TryGetValue(identity, out var observerData))
            {
                if (identityData.Get() == observerData.Get())
                    return true;
            }*/
            return false;
        }

        public bool VisibleByDistance(Transform first, Transform second)
        {
            if (EnableLogging)
                print($"Distance: {first}, {second}");

            return Vector3.Distance(first.position, second.position) < MaxDistance;
        }

        public bool VisibleByLinecast(Transform first, Transform second)
        {
            if (EnableLogging)
                print($"Linecast: {first}, {second}");

            return !Physics.Linecast(first.position, second.position, RaycastLayerMask);
        }

        public bool VisibleByPrediction(Transform first, Transform second)
        {
            if (EnableLogging)
                print($"Prediction: {first}, {second}");

            Vector3 directionToSecond = second.position - first.position;
            Vector3? directionToWall = null;

            // Same as Vector3.Distance
            float distance = directionToSecond.magnitude;

            if (DrawGizmos)
                gizmosDirections.Clear();

            Vector3 eulers = Quaternion.LookRotation(directionToSecond).eulerAngles;

            for (int y = 0; y < VerticalRaysCount; y++)
            {
                for (int x = 0; x < HorizontalRaysCount; x++)
                {
                    var eulerX = eulers.x + (VerticalRaysSpace * GetDirectionMultiplier(y, VerticalRaysDirection) * y);
                    var eulerY = eulers.y + (HorizontalRaysSpace * GetDirectionMultiplier(x, HorizontalRaysDirection) * x);
                    Vector3 direction = Quaternion.Euler(eulerX, eulerY, eulers.z) * Vector3.forward;

                    if (DrawGizmos)
                        gizmosDirections.Add(direction);

                    if (!Physics.Raycast(first.position, direction, distance, RaycastLayerMask))
                    {
                        directionToWall = direction;
                        break;
                    }
                }

                if (directionToWall.HasValue)
                    break;
            }

            if (!directionToWall.HasValue)
                return false;

            Vector3 project = Vector3.Project(directionToWall.Value, directionToSecond);

            if (DrawGizmos)
            {
                gizmosIdentity = first.position;
                gizmosDirectionToSecond = directionToSecond;
                gizmosDirectionToWall = directionToWall.Value;
            }

            return Vector3.Distance(directionToSecond, project) < MaxPredictionDistance;
        }

        private float GetDirectionMultiplier(int index, RaysDirection direction)
        {
            return direction switch
            {
                RaysDirection.Positive => 1f,
                RaysDirection.Negative => 0f,
                RaysDirection.Alternating => index % 2 == 0 ? 1 : -1,
                _ => throw new NotImplementedException()
            };
        }

        #endregion

        #region Debug

        protected virtual void OnDrawGizmosSelected()
        {
            if (!DrawGizmos)
                return;

            Vector3 project = Vector3.Project(gizmosDirectionToWall, gizmosDirectionToSecond);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(gizmosIdentity, gizmosDirectionToSecond);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(gizmosIdentity, gizmosDirectionToWall);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(gizmosIdentity + gizmosDirectionToSecond, project);

            Gizmos.color = Color.cyan;
            foreach (var dir in gizmosDirections)
            {
                Gizmos.DrawRay(gizmosIdentity, dir * gizmosDirectionToSecond.magnitude);
            }
        }

        #endregion
    }
}