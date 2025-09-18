using UnityEngine;
using UnityEngine.UI;

namespace Breaddog.UI
{
    public class InventoryDrawerCell : MonoBehaviour
    {
        public RectTransform Rect;
        public Image CellImage;
        public Color FreeColor = Color.white;
        public Color PlacedColor = Color.gray7;
        public Color DisabledColor = Color.gray5;

        private InventoryCellState currentState;


        public virtual void Initialize()
        {

        }

        public virtual void SetState(InventoryCellState state)
        {
            currentState = state;

            CellImage.color = currentState switch
            {
                InventoryCellState.Free => FreeColor,
                InventoryCellState.Placed => PlacedColor,
                InventoryCellState.Disabled => DisabledColor,
                _ => Color.magenta
            };
        }
    }

    public enum InventoryCellState
    {
        Free,
        Placed,
        Disabled
    }
}
