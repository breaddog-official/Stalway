using Breaddog.Extensions;
using Breaddog.Gameplay.StorageManagement;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Breaddog.Network
{
    public class SyncStorage : SyncObject
    {
        public Action OnPlace;

        public Action OnRemove;

        public Action OnReplace;

        public Action OnResize;

        public Action OnClear;

        public enum Operation : byte
        {
            OP_PLACE,
            OP_REMOVE,
            OP_REPLACE,
            OP_RESIZE,
            OP_CLEAR,
        }

        public Action<Operation> OnChange;

        public readonly Storage storage = new(0, 0);
        public bool IsReadOnly => !IsWritable();

        struct Change
        {
            internal Operation operation;

            internal int index;
            internal Item item;
            internal Vector2Int position;
            internal Rotation4 rotation;

            internal int width;
            internal int height;
        }

        // list of changes.
        // -> insert/delete/clear is only ONE change
        // -> changing the same slot 10x caues 10 changes.
        // -> note that this grows until next sync(!)
        // TODO Dictionary<key, change> to avoid ever growing changes / redundant changes!
        readonly List<Change> changes = new List<Change>();

        // how many changes we need to ignore
        // this is needed because when we initialize the list,
        // we might later receive changes that have already been applied
        // so we need to skip them
        int changesAhead;


        public override void Reset()
        {
            changes.Clear();
            changesAhead = 0;
            storage.Clear();
        }

        // throw away all the changes
        // this should be called after a successful sync
        public override void ClearChanges() => changes.Clear();

        void AddOperation(Operation op, bool checkAccess, Item item = null, int index = -1, Vector2Int position = default, Rotation4 rotation = default, int width = -1, int height = -1)
        {
            if (checkAccess && IsReadOnly)
                throw new InvalidOperationException("SyncSets can only be modified by the owner.");

            Change change = default;
            switch (op)
            {
                case Operation.OP_PLACE:
                    change = new Change
                    {
                        operation = op,
                        item = item,
                        position = position,
                        rotation = rotation
                    };
                    break;
                case Operation.OP_REMOVE:
                    change = new Change
                    {
                        operation = op,
                        index = index,
                    };
                    break;
                case Operation.OP_REPLACE:
                    change = new Change
                    {
                        operation = op,
                        index = index,
                        position = position,
                        rotation = rotation
                    };
                    break;
                case Operation.OP_RESIZE:
                    change = new Change
                    {
                        operation = op,
                        width = width,
                        height = height,
                    };
                    break;
                case Operation.OP_CLEAR:
                    change = new Change
                    {
                        operation = op
                    };
                    break;
            }

            if (IsRecording())
            {
                changes.Add(change);
                OnDirty?.Invoke();
            }

            switch (op)
            {
                case Operation.OP_PLACE:
                    OnPlace?.Invoke();
                    break;
                case Operation.OP_REMOVE:
                    OnRemove?.Invoke();
                    break;
                case Operation.OP_REPLACE:
                    OnReplace?.Invoke();
                    break;
                case Operation.OP_RESIZE:
                    OnResize?.Invoke();
                    break;
                case Operation.OP_CLEAR:
                    OnClear?.Invoke();
                    break;
            }

            OnChange?.Invoke(op);
        }

        void AddOperation(Operation op, bool checkAccess) => AddOperation(op, checkAccess);

        public override void OnSerializeAll(NetworkWriter writer)
        {
            writer.WriteReadonlyArray2D(storage.Places);
            writer.WriteReadonlyList(storage.Items);

            // all changes have been applied already
            // thus the client will need to skip all the pending changes
            // or they would be applied again.
            // So we write how many changes are pending
            writer.WriteUInt((uint)changes.Count);
        }

        public override void OnSerializeDelta(NetworkWriter writer)
        {
            // write all the queued up changes
            writer.WriteUInt((uint)changes.Count);

            for (int i = 0; i < changes.Count; i++)
            {
                Change change = changes[i];
                writer.WriteByte((byte)change.operation);

                switch (change.operation)
                {
                    case Operation.OP_PLACE:
                        writer.Write(change.item);
                        writer.WriteVector2Int(change.position);
                        writer.WriteByte((byte)change.rotation);
                        break;

                    case Operation.OP_REMOVE:
                        Compression.CompressVarInt(writer, change.index);
                        break;

                    case Operation.OP_REPLACE:
                        Compression.CompressVarInt(writer, change.index);
                        writer.WriteVector2Int(change.position);
                        writer.WriteByte((byte)change.rotation);
                        break;

                    case Operation.OP_RESIZE:
                        Compression.CompressVarInt(writer, change.width);
                        Compression.CompressVarInt(writer, change.height);
                        break;
                }
            }
        }

        public override void OnDeserializeAll(NetworkReader reader)
        {
            changes.Clear();

            var places = reader.ReadArray2D<int>();
            var items = reader.ReadList<StoredItem>();

            storage.SetPlaces(places);
            storage.SetItems(items);

            // We will need to skip all these changes
            // the next time the list is synchronized
            // because they have already been applied
            changesAhead = (int)reader.ReadUInt();
        }

        public override void OnDeserializeDelta(NetworkReader reader)
        {
            int changesCount = (int)reader.ReadUInt();

            for (int i = 0; i < changesCount; i++)
            {
                Operation operation = (Operation)reader.ReadByte();

                // apply the operation only if it is a new change
                // that we have not applied yet
                bool apply = changesAhead == 0;

                Item item;
                Vector2Int pos;
                Rotation4 rot;
                int index;
                int width;
                int height;

                switch (operation)
                {
                    case Operation.OP_PLACE:
                        item = reader.Read<Item>();
                        pos = reader.ReadVector2Int();
                        rot = (Rotation4)reader.ReadByte();

                        if (apply)
                        {
                            storage.PlaceItem(item, pos, rot);
                            // add dirty + changes.
                            // ClientToServer needs to set dirty in server OnDeserialize.
                            // no access check: server OnDeserialize can always
                            // write, even for ClientToServer (for broadcasting).
                            AddOperation(Operation.OP_PLACE, false, item: item, position: pos, rotation: rot);
                        }
                        break;

                    case Operation.OP_REMOVE:
                        index = (int)Compression.DecompressVarInt(reader);

                        if (apply)
                        {
                            storage.RemoveItem(index);
                            // add dirty + changes.
                            // ClientToServer needs to set dirty in server OnDeserialize.
                            // no access check: server OnDeserialize can always
                            // write, even for ClientToServer (for broadcasting).
                            AddOperation(Operation.OP_REMOVE, false, index: index);
                        }
                        break;

                    case Operation.OP_REPLACE:
                        index = (int)Compression.DecompressVarInt(reader);
                        pos = reader.ReadVector2Int();
                        rot = (Rotation4)reader.ReadByte();

                        if (apply)
                        {
                            storage.ReplaceItem(index, pos, rot);
                            // add dirty + changes.
                            // ClientToServer needs to set dirty in server OnDeserialize.
                            // no access check: server OnDeserialize can always
                            // write, even for ClientToServer (for broadcasting).
                            AddOperation(Operation.OP_REPLACE, false, index: index, position: pos, rotation: rot);
                        }
                        break;

                    case Operation.OP_RESIZE:
                        width = (int)Compression.DecompressVarInt(reader);
                        height = (int)Compression.DecompressVarInt(reader);

                        if (apply)
                        {
                            storage.Resize(width, height);
                            // add dirty + changes.
                            // ClientToServer needs to set dirty in server OnDeserialize.
                            // no access check: server OnDeserialize can always
                            // write, even for ClientToServer (for broadcasting).
                            AddOperation(Operation.OP_RESIZE, false, width: width, height: height);
                        }
                        break;

                    case Operation.OP_CLEAR:
                        if (apply)
                        {
                            // add dirty + changes.
                            // ClientToServer needs to set dirty in server OnDeserialize.
                            // no access check: server OnDeserialize can always
                            // write, even for ClientToServer (for broadcasting).
                            AddOperation(Operation.OP_CLEAR, false);
                            // clear after invoking the callback so users can iterate the set
                            // and take appropriate action on the items before they are wiped.
                            storage.Clear();
                        }
                        break;
                }

                if (!apply)
                {
                    // we just skipped this change
                    changesAhead--;
                }
            }
        }




        public int PlaceItem(Item item, Vector2Int position, Rotation4 rotation)
        {
            int index = storage.PlaceItem(item, position, rotation);

            if (index > Storage.defaultIndex)
            {
                AddOperation(Operation.OP_PLACE, true, item: item, position: position, rotation: rotation);
            }

            return index;
        }

        public bool Remove(int index)
        {
            if (storage.RemoveItem(index))
            {
                AddOperation(Operation.OP_REMOVE, true, index: index);
                return true;
            }
            return false;
        }

        public bool ReplaceItem(int itemIndex, Vector2Int position, Rotation4 rotation)
        {
            if (storage.ReplaceItem(itemIndex, position, rotation))
            {
                AddOperation(Operation.OP_REMOVE, true, index: itemIndex, position: position, rotation: rotation);
                return true;
            }
            return false;
        }




        public int TryPlaceItem(Item item) => TryPlaceItem(item, out _, out _);
        public int TryPlaceItem(Item item, out Rotation4 rotation) => TryPlaceItem(item, out _, out rotation);
        public int TryPlaceItem(Item item, out Vector2Int position) => TryPlaceItem(item, out position, out _);
        public int TryPlaceItem(Item item, out Vector2Int position, out Rotation4 rotation)
        {
            int index = storage.TryPlaceItem(item, out position, out rotation);

            if (index > Storage.defaultIndex)
            {
                AddOperation(Operation.OP_PLACE, true, item: item, position: position, rotation: rotation);
            }

            return index;
        }




        public void Resize(int width, int height)
        {
            storage.Resize(width, height);

            AddOperation(Operation.OP_RESIZE, true, width: width, height: height);
        }

        public void Clear()
        {
            AddOperation(Operation.OP_CLEAR, true);
            // clear after invoking the callback so users can iterate the set
            // and take appropriate action on the items before they are wiped.
            storage.Clear();
        }
    }
}