using UnityEngine;
using UnityEngine.UI;

namespace Breaddog.UI
{
    [ExecuteAlways]
    public class LayoutPrefferedSize : MonoBehaviour
    {
        public LayoutGroup Group;
        [Space]
        public bool OverrideX;
        public RectTransform xRect;
        public bool OverrideY;
        public RectTransform yRect;

        private LayoutGroupOptimizator optimizator;


        private void Start()
        {
            optimizator = GetComponent<LayoutGroupOptimizator>();

            if (OverrideX && xRect == null)
                xRect = GetComponent<RectTransform>();

            if (OverrideY && yRect == null)
                yRect = GetComponent<RectTransform>();

            if (Group == null)
                Group = GetComponent<LayoutGroup>();
        }

        private void Update()
        {
            if (Group == null || !Group.enabled)
                return;

            if (OverrideX && xRect == null)
                return;

            if (OverrideY && yRect == null)
                return;

            var x = OverrideX ? Group.preferredWidth : xRect.sizeDelta.x;
            var y = OverrideY ? Group.preferredHeight : yRect.sizeDelta.y;

            if (optimizator != null && Application.isPlaying)
                optimizator.UpdateGroup();

            xRect.sizeDelta = new(x, xRect.sizeDelta.y);
            yRect.sizeDelta = new(yRect.sizeDelta.x, y);
        }
    }
}
