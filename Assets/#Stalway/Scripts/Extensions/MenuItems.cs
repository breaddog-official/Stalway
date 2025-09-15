using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Breaddog.Extensions
{
    public static class MenuItems
    {
        private const string CORRECT_COLLIDER_CENTER = "Tools/Correct Collider Center";

#if UNITY_EDITOR
        [MenuItem(CORRECT_COLLIDER_CENTER, validate = true)]
        public static bool CorrectColliderCenterValidation()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<Collider>(out _);
        }

        [MenuItem(CORRECT_COLLIDER_CENTER)]
        public static void CorrectColliderCenter()
        {
            var transform = Selection.activeGameObject.transform;

            if (Selection.activeGameObject.TryGetComponent(out BoxCollider box))
            {
                transform.position = transform.TransformPoint(box.center);
                box.center = Vector3.zero;
            }
            else if (Selection.activeGameObject.TryGetComponent(out SphereCollider sphere))
            {
                transform.position = transform.TransformPoint(sphere.center);
                sphere.center = Vector3.zero;
            }
            else if (Selection.activeGameObject.TryGetComponent(out CapsuleCollider capsule))
            {
                transform.position = transform.TransformPoint(capsule.center);
                capsule.center = Vector3.zero;
            }
            else if (Selection.activeGameObject.TryGetComponent(out WheelCollider wheel))
            {
                transform.position = transform.TransformPoint(wheel.center);
                wheel.center = Vector3.zero;
            }
        }
#endif
    }
}