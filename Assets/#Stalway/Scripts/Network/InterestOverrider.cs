using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Breaddog.Network
{
    public class InterestOverrider : SerializedMonoBehaviour, IInterestOverrider
    {
        [OdinSerialize] public Transform InterestTransform { get; }
    }

    public interface IInterestOverrider
    {
        public Transform InterestTransform { get; }
    }
}