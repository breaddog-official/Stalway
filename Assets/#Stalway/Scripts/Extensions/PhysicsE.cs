using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    [BurstCompile]
    public static class PhysicsE
    {
        #region OverlapCollider & CheckCollider

        public static Collider[] OverlapCollider(this Collider collider, int layers = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, Vector3 centerOffset = default)
        {
            if (collider is SphereCollider sphere)
            {
                var center = sphere.transform.TransformPoint(sphere.center) + centerOffset;
                return Physics.OverlapSphere(center, sphere.radius, layers, queryTriggerInteraction);
            }
            else if (collider is WheelCollider wheel)
            {
                var center = wheel.transform.TransformPoint(wheel.center) + centerOffset;
                return Physics.OverlapSphere(center, wheel.radius, layers, queryTriggerInteraction);
            }
            else if (collider is CapsuleCollider capsule)
            {
                var direction = new Vector3 { [capsule.direction] = 1 };
                var offset = capsule.height / 2 - capsule.radius;
                var point0 = capsule.transform.TransformPoint(capsule.center - direction * offset) + centerOffset;
                var point1 = capsule.transform.TransformPoint(capsule.center + direction * offset) + centerOffset;
                return Physics.OverlapCapsule(point0, point1, capsule.radius, layers, queryTriggerInteraction);
            }
            else if (collider is BoxCollider box)
            {
                Vector3 worldCenter = box.transform.TransformPoint(box.center) + centerOffset;
                Vector3 worldHalfExtents = box.transform.TransformVector(box.size * 0.5f);
                return Physics.OverlapBox(worldCenter, worldHalfExtents, box.transform.rotation, layers, queryTriggerInteraction);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static int OverlapColliderNonAlloc(this Collider collider, ref Collider[] colliders, int layers = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, Vector3 centerOffset = default)
        {
            if (collider is SphereCollider sphere)
            {
                var center = sphere.transform.TransformPoint(sphere.center) + centerOffset;
                return Physics.OverlapSphereNonAlloc(center, sphere.radius, colliders, layers, queryTriggerInteraction);
            }
            else if (collider is WheelCollider wheel)
            {
                var center = wheel.transform.TransformPoint(wheel.center) + centerOffset;
                return Physics.OverlapSphereNonAlloc(center, wheel.radius, colliders, layers, queryTriggerInteraction);
            }
            else if (collider is CapsuleCollider capsule)
            {
                var direction = new Vector3 { [capsule.direction] = 1 };
                var offset = capsule.height / 2 - capsule.radius;
                var point0 = capsule.transform.TransformPoint(capsule.center - direction * offset) + centerOffset;
                var point1 = capsule.transform.TransformPoint(capsule.center + direction * offset) + centerOffset;
                return Physics.OverlapCapsuleNonAlloc(point0, point1, capsule.radius, colliders, layers, queryTriggerInteraction);
            }
            else if (collider is BoxCollider box)
            {
                Vector3 worldCenter = box.transform.TransformPoint(box.center) + centerOffset;
                Vector3 worldHalfExtents = box.transform.TransformVector(box.size * 0.5f);
                return Physics.OverlapBoxNonAlloc(worldCenter, worldHalfExtents, colliders, box.transform.rotation, layers, queryTriggerInteraction);
            }
            else
            {
                throw new NotImplementedException();
            }
        }




        public static bool CheckCollider(this Collider collider, int layers = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal, Vector3 centerOffset = default, float tolerance = 0f)
        {
            var toleranceMultiplier = 1f - tolerance;
            var toleranceVector = new Vector3(toleranceMultiplier, toleranceMultiplier, toleranceMultiplier);

            if (collider is SphereCollider sphere)
            {
                var center = sphere.transform.TransformPoint(sphere.center) + centerOffset;
                return Physics.CheckSphere(center, sphere.radius * toleranceMultiplier, layers, queryTriggerInteraction);
            }
            else if (collider is WheelCollider wheel)
            {
                var center = wheel.transform.TransformPoint(wheel.center) + centerOffset;
                return Physics.CheckSphere(center, wheel.radius * toleranceMultiplier, layers, queryTriggerInteraction);
            }
            else if (collider is CapsuleCollider capsule)
            {
                var direction = new Vector3 { [capsule.direction] = 1 } * toleranceMultiplier;
                var radius = capsule.radius * toleranceMultiplier;
                var offset = capsule.height / 2 - radius;
                var point0 = capsule.transform.TransformPoint(capsule.center - direction * offset) + centerOffset;
                var point1 = capsule.transform.TransformPoint(capsule.center + direction * offset) + centerOffset;
                return Physics.CheckCapsule(point0, point1, radius, layers, queryTriggerInteraction);
            }
            else if (collider is BoxCollider box)
            {
                Vector3 worldCenter = box.transform.TransformPoint(box.center) + centerOffset;
                Vector3 worldHalfExtents = box.transform.TransformVector(Vector3.Scale(box.size, toleranceVector) * 0.5f);
                return Physics.CheckBox(worldCenter, worldHalfExtents, box.transform.rotation, layers, queryTriggerInteraction);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static void GizmosCollider(this Collider collider, Vector3 centerOffset = default, float tolerance = 0f)
        {
            var toleranceMultiplier = 1f - tolerance;
            var toleranceVector = new Vector3(toleranceMultiplier, toleranceMultiplier, toleranceMultiplier);
            var capsuleQuality = 7;

            if (collider is SphereCollider sphere)
            {
                var center = sphere.transform.TransformPoint(sphere.center) + centerOffset;
                Gizmos.DrawSphere(center, sphere.radius * toleranceMultiplier);
            }
            else if (collider is WheelCollider wheel)
            {
                var center = wheel.transform.TransformPoint(wheel.center) + centerOffset;
                Gizmos.DrawSphere(center, wheel.radius * toleranceMultiplier);
            }
            else if (collider is CapsuleCollider capsule)
            {
                var direction = new Vector3 { [capsule.direction] = 1 };
                var radius = capsule.radius * toleranceMultiplier;
                var offset = capsule.height / 2 - radius;
                var point0 = capsule.transform.TransformPoint(capsule.center - direction * offset) + centerOffset;
                var point1 = capsule.transform.TransformPoint(capsule.center + direction * offset) + centerOffset;
                //Gizmos.DrawSphere(point0, radius);
                //Gizmos.DrawSphere(point1, radius);
                for (float i = 0; i < capsuleQuality; i++)
                {
                    Gizmos.DrawSphere(Vector3.Lerp(point0, point1, i / capsuleQuality), radius);
                }
            }
            else if (collider is BoxCollider box)
            {
                Vector3 worldCenter = box.transform.TransformPoint(box.center) + centerOffset;
                Vector3 worldHalfExtents = box.transform.TransformVector(Vector3.Scale(box.size, toleranceVector) * 0.5f);
                Gizmos.DrawCube(worldCenter, worldHalfExtents);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region CapsuleCollider Helpers

        public static Vector3 GetZeroPoint(this CapsuleCollider collider)
        {
            var direction = new Vector3 { [collider.direction] = 1 };
            var offset = collider.height / 2 - collider.radius;
            return collider.center - direction * offset;
        }

        public static Vector3 GetFirstPoint(this CapsuleCollider collider)
        {
            var direction = new Vector3 { [collider.direction] = 1 };
            var offset = collider.height / 2 - collider.radius;
            return collider.center + direction * offset;
        }

        #endregion

        #region LinecastAll

        public static RaycastHit[] LinecastAll(Vector3 start, Vector3 end, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.RaycastAll(start, end - start, Vector3.Distance(start, end), layerMask, queryTriggerInteraction);
        }

        public static int LinecastNonAlloc(Vector3 start, Vector3 end, RaycastHit[] results, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            return Physics.RaycastNonAlloc(start, end - start, results, Vector3.Distance(start, end), layerMask, queryTriggerInteraction);
        }

        #endregion
    }
}
