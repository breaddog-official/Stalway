using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    [BurstCompile]
    public static class GameplayE
    {
        #region Teleportate

        /// <summary>
        /// Teleportates gameobject via Rigidbody or Transform
        /// </summary>
        public static void Teleportate(this GameObject gameObject, Vector3 position, Quaternion rotation)
        {
            // Try get predicted rigidbody and move them
            //if (gameObject.TryGetComponent<PredictedRigidbody>(out var predictedRb))
            //    predictedRb.predictedRigidbody.Move(position, rotation);

            // Try get rigidbody and move them
            if (gameObject.TryGetComponent<Rigidbody>(out var rb))
                rb.Move(position, rotation);

            // Otherwise, move via transform
            else
                gameObject.transform.SetPositionAndRotation(position, rotation);
        }

        /// <summary>
        /// Teleportates gameobject via Rigidbody or Transform. If ignoreRotation is true, gameObject will not be rotated
        /// </summary>
        public static void Teleportate(this GameObject gameObject, Transform point, bool ignoreRotation = false)
            => gameObject.Teleportate(point.position, ignoreRotation ? gameObject.transform.rotation : point.rotation);

        #endregion

        #region DontDestroyOnLoad

        /// <summary>
        /// Sets object DontDestryOnLoad and if needs, moves to the root directory
        /// </summary>
        public static void DontDestroyOnLoad(this GameObject gameObject)
        {
            // DontDestroyOnLoad dont work with childrens
            if (gameObject.transform.parent != null)
                gameObject.transform.parent = null;

            UnityEngine.Object.DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region DisableAfter

        /// <summary>
        /// Disables gameObject after duration
        /// </summary>
        public static async UniTask DisableAfter(this GameObject gameObject, float duration, CancellationToken token = default)
        {
            await UniTask.Delay(duration.ConvertSecondsToMiliseconds(), cancellationToken: token);

            if (gameObject != null)
                gameObject.SetActive(false);
        }

        #endregion
    }
}
