using Breaddog.Extensions;
using Breaddog.Gameplay;
using Breaddog.Gameplay.StorageManagement;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace Breaddog.UI
{
    public class InventoryDrawer : MonoBehaviour
    {
        [Range(0, 20)]
        public uint UpdateFrames = 5;
        public bool UpdateAsync = true;
        public Transform CellsParent;
        public Transform ItemsParent;
        public InventoryDrawerCell CellPrefab;
        public InventoryDrawerItem ItemPrefab;
        [Space, FromResources]
        public string TestItem;

        /// <summary>
        /// Expensive
        /// </summary>
        public Storage ObservingStorage => Entity.ObservingEntity?.FindAbillity<AbillityInventory>().Backpack.storage;
        private uint currentFrames;
        private GridLayoutGroup cellsGrid;

        private readonly List<InventoryDrawerCell> cells = new(64);
        private readonly List<InventoryDrawerItem> items = new(64);
        private CancellationTokenSource token;
        private bool updating;


        private void OnEnable()
        {
            token?.ResetToken();
            token = new();
        }

        private void OnDisable()
        {
            token?.Cancel();
        }
        private void Update()
        {
            currentFrames++;
            if (currentFrames > UpdateFrames && !updating)
            {
                currentFrames = 0;
                UpdateInventory().Forget();
            }
        }

        private async UniTask UpdateInventory()
        {
            // Cache
            var storage = ObservingStorage;

            if (storage == null)
            {
                ClearInventory();
                return;
            }

            updating = true;

            try
            {
                // Cells
                if (cellsGrid != null || CellsParent.TryGetComponent(out cellsGrid))
                {
                    cellsGrid.constraint = storage.Width > storage.Height ? GridLayoutGroup.Constraint.FixedColumnCount : GridLayoutGroup.Constraint.FixedRowCount;
                    cellsGrid.constraintCount = storage.MaxSide;
                }

                var placesCount = storage.Places.Count;

                while (cells.Count < placesCount)
                {
                    var spawnedCell = Instantiate(CellPrefab, CellsParent);
                    spawnedCell.gameObject.SetActive(true);
                    spawnedCell.Initialize();
                    cells.Add(spawnedCell);
                }

                while (cells.Count > placesCount)
                {
                    var index = cells.Count - 1;
                    Destroy(cells[index]);
                    cells.RemoveAt(index);
                }

                for (int i = 0; i < placesCount; i++)
                {
                    cells[i].SetState(storage.Places.RawData[i] == Storage.defaultIndex ? InventoryCellState.Free : InventoryCellState.Placed);
                }

                // Items
                var itemsCount = storage.Items.Count;
                var itemI = 0;

                while (items.Count < itemsCount)
                {
                    var spawnedItem = Instantiate(ItemPrefab, ItemsParent);
                    spawnedItem.gameObject.SetActive(true);
                    spawnedItem.Initialize(async: UpdateAsync);
                    items.Add(spawnedItem);

                    itemI++;
                }

                while (items.Count > itemsCount)
                {
                    var index = items.Count - 1;
                    Destroy(items[index]);
                    items.RemoveAt(index);
                }

                for (int i = 0; i < itemsCount; i++)
                {
                    var update = items[i].UpdateItem(storage.Items[i], token.Token);
                    if (UpdateAsync)
                        await update;

                    UpdatePosition(items[i], storage);
                }
            }
            finally
            {
                updating = false;
            }
        }

        protected virtual void UpdatePosition(InventoryDrawerItem item, Storage storage = null)
        {
            storage ??= ObservingStorage;

            var shape = item.CurrentItem.itemAsset.Shape;
            var rotatedSize = shape.RotatedSize(item.CurrentItem.rotation);
            var pos = item.CurrentItem.position;

            item.Rect.sizeDelta = cellsGrid.cellSize * new Vector2(shape.Width, shape.Height);

            Vector2 total = Vector2.zero;
            int count = 0;

            for (int y = 0; y < rotatedSize.y; y++) // Height
                for (int x = 0; x < rotatedSize.x; x++) // Width
                {
                    var cell = GetCell(pos.x + x, pos.y + y, storage);
                    total += cell.Rect.anchoredPosition;
                    count++;
                }

            if (count > 0)
                item.Rect.anchoredPosition = total / count;

            var zRot = item.CurrentItem.rotation switch
            {
                Rotation4.Up => 0f,
                Rotation4.Right => 90f,
                Rotation4.Down => 180f,
                Rotation4.Left => 270f,
                _ => 0f
            };

            item.Rect.localRotation = Quaternion.Euler(0f, 0f, zRot);
        }


        public InventoryDrawerCell GetCell(int x, int y, Storage storage = null)
        {
            storage ??= ObservingStorage;
            return cells[y * storage.Width + x];
        }

        private void ClearInventory()
        {
            while (cells.Count > 0)
            {
                var index = cells.Count - 1;

                Destroy(cells[index].gameObject);
                cells.RemoveAt(index);
            }

            while (items.Count > 0)
            {
                var index = items.Count - 1;

                Destroy(items[index].gameObject);
                items.RemoveAt(index);
            }

            if (cellsGrid != null)
            {
                cellsGrid.constraint = GridLayoutGroup.Constraint.Flexible;
                cellsGrid.constraintCount = 0;
            }
        }

        [Button]
        public void AddTestItem()
        {
            if (string.IsNullOrWhiteSpace(TestItem))
                return;

            if (ObservingStorage == null)
                return;

            var item = Item.Create(TestItem);
            ObservingStorage.TryPlaceItem(item);
        }
    }
}
