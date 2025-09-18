using UnityEngine;

namespace Breaddog.Network
{
    public class InterestOverrider : MonoBehaviour, IInterestOverrider
    {
        public Transform interestTransform;
        public Transform InterestTransform => interestTransform;
    }

    public interface IInterestOverrider
    {
        public Transform InterestTransform { get; }
    }
}