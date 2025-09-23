using UnityEngine;
using UnityEngine.UI;

namespace Breaddog.UI
{
    public class StorageDrawerCell : MonoBehaviour
    {
        public RectTransform Rect;
        public Image CellImage;
        public Color FreeColor = Color.white;
        public Color PlacedColor = Color.gray7;
        public Color DisabledColor = Color.gray5;

        public StorageCellState CurrentState { get; protected set; }


        public virtual void Initialize(StorageCellState state = StorageCellState.Free)
        {
            SetState(CurrentState);
        }

        public virtual void SetState(StorageCellState state)
        {
            CurrentState = state;

            CellImage.color = CurrentState switch
            {
                StorageCellState.Free => FreeColor,
                StorageCellState.Placed => PlacedColor,
                StorageCellState.Disabled => DisabledColor,
                _ => Color.magenta
            };
        }
    }

    public enum StorageCellState
    {
        Free,
        Placed,
        Disabled
    }
}
