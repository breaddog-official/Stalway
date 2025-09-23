using Breaddog.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [BurstCompile, Serializable]
    public partial class Storage
    {
        private Array2D<int> places; // Array of indexes in Items
        private List<StoredItem> items;

        public const int defaultIndex = -1;


        public int Width => places.Width;
        public int Height => places.Height;
        public IReadOnlyArray2D<int> Places => places;
        public IReadOnlyList<StoredItem> Items => items;
        public int MaxSide => Math.Max(Width, Height);
        public int MinSide => Math.Min(Width, Height);
        public int Square => Width * Height;



        public Storage()
        {
            places = new(0, 0);
            items = new();
        }

        public Storage(int width, int height)
        {
            places = new(width, height);
            items = new(Square / 2);

            places.Fill((x, y) => defaultIndex);
        }

        public Storage(IReadOnlyArray2D<int> placesCollection, IReadOnlyCollection<StoredItem> itemsCollection)
        {
            places = new(placesCollection);
            items = new(itemsCollection);
        }






        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StoredItem GetItem(int x, int y)
        {
            return items[places[x, y]];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(int x, int y)
        {
            return (uint)x < places.Width && (uint)y < places.Height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InBounds(int itemIndex)
        {
            return (uint)itemIndex < items.Count;
        }





        public int PlaceItem(Item item, Vector2Int position, Rotation4 rotation)
        {
            return PlaceItem(item, position, rotation, false);
        }

        private int PlaceItem(Item item, Vector2Int position, Rotation4 rotation, bool skipPlaceCheck)
        {
            var shape = item.Shape.RotateShape(rotation);

            if (!skipPlaceCheck && !CanPlace(shape, position))
            {
                return defaultIndex;
            }

            var storedItem = new StoredItem(item, position, rotation, shape);

            items.Add(storedItem);
            var index = items.Count - 1;

            Fill(shape, position, index);

            return index;
        }

        public bool RemoveItem(int itemIndex)
        {
            if (InBounds(itemIndex) == false)
                return false;

            var item = items[itemIndex];
            var shape = item.itemAsset.Shape.RotateShape(item.rotation);

            Fill(shape, item.position, defaultIndex);

            items.RemoveAt(itemIndex);

            return true;
        }

        public bool ReplaceItem(int itemIndex, Vector2Int position, Rotation4 rotation)
        {
            if (InBounds(itemIndex) == false)
                return false;

            var item = items[itemIndex];
            var oldShape = item.cachedShape;
            var newShape = item.rotation != rotation ?
                           item.itemAsset.Shape.RotateShape(rotation) : oldShape;

            if (CanPlace(newShape, position) == false)
                return false;

            // Remove
            Fill(oldShape, item.position, defaultIndex);

            // Place
            Fill(newShape, position, itemIndex);

            item.Set(position, rotation);

            return true;
        }




        public int TryPlaceItem(Item item) => TryPlaceItem(item, out _, out _);
        public int TryPlaceItem(Item item, out Rotation4 rotation) => TryPlaceItem(item, out _, out rotation);
        public int TryPlaceItem(Item item, out Vector2Int position) => TryPlaceItem(item, out position, out _);
        public int TryPlaceItem(Item item, out Vector2Int position, out Rotation4 rotation)
        {
            for (int rot = 0; rot < 4; rot++)
            {
                var enumRot = (Rotation4)rot;
                var rotatedShape = item.Shape.RotateShape(enumRot);

                for (int y = 0; y <= Height - rotatedShape.Height; y++)
                {
                    for (int x = 0; x <= Width - rotatedShape.Width; x++)
                    {
                        var pos = new Vector2Int(x, y);
                        if (CanPlace(rotatedShape, pos))
                        {
                            int index = PlaceItem(item, pos, enumRot, skipPlaceCheck: true);
                            position = pos;
                            rotation = enumRot;
                            return index;
                        }
                    }
                }
            }

            position = default;
            rotation = default;
            return defaultIndex;
        }




        public void Resize(int newWidth, int newHeight)
        {
            if (places.Width == newWidth && places.Height == newHeight)
                return;

            places.ResizeAndFill(newWidth, newHeight, defaultIndex);
        }

        public void Clear()
        {
            places.Fill((x, y) => defaultIndex);
            items.Clear();
        }




        internal void SetPlaces(Array2D<int> places)
        {
            this.places = places;
        }

        internal void SetItems(List<StoredItem> items)
        {
            this.items = items;
        }




        public bool CanPlace(Array2D<bool> shape, Vector2Int pos)
        {
            for (int dy = 0; dy < shape.Height; dy++)
                for (int dx = 0; dx < shape.Width; dx++)
                    if (shape[dx, dy] && (!InBounds(pos.x + dx, pos.y + dy) || places[pos.x + dx, pos.y + dy] != defaultIndex))
                        return false;

            return true;
        }

        private void Fill(Array2D<bool> shape, Vector2Int pos, int index)
        {
            for (int dy = 0; dy < shape.Height; dy++)
                for (int dx = 0; dx < shape.Width; dx++)
                    if (shape[dx, dy])
                        places[pos.x + dx, pos.y + dy] = index;
        }
    }

    public class StoredItem
    {
        public Item itemAsset;
        public Vector2Int position;
        public Rotation4 rotation;

        public Array2D<bool> cachedShape;



        public StoredItem(Item item, Vector2Int position, Rotation4 rotation)
        {
            this.itemAsset = item;
            this.position = position;
            this.rotation = rotation;
            RecalculateShape();
        }

        public StoredItem(Item item, Vector2Int position, Rotation4 rotation, Array2D<bool> shape)
        {
            this.itemAsset = item;
            this.position = position;
            this.rotation = rotation;
            this.cachedShape = shape;
        }



        public void Set(Rotation4 rotation)
        {
            if (this.rotation != rotation)
            {
                this.rotation = rotation;
                RecalculateShape();
            }
        }

        public void Set(Vector2Int position, Rotation4 rotation)
        {
            this.position = position;
            Set(rotation);
        }

        private void RecalculateShape()
            => cachedShape = itemAsset.Shape.RotateShape(rotation);
    }
}