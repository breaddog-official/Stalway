using UnityEngine;

// Некоторые функции аниматора не работают без галочки root motion (например pivotWeight)
public class RootMotionNeutralizer : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
