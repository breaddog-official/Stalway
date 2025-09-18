using Breaddog.Gameplay.StorageManagement;
using Cysharp.Threading.Tasks;
using Mirror;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Unity.Burst;
using UnityEngine;

namespace Breaddog.Extensions
{
    public enum MoveAuthority
    {
        ClientAuthority,
        ServerAuthority,
        Prediction,
        Hybrid
    }

    [BurstCompile]
    public static class NetworkE
    {
        #region FindByUid Identity

        /// <summary>
        /// Finds identity by id
        /// </summary>
        public static NetworkIdentity FindByID(this uint ID)
        {
            return NetworkClient.spawned.GetValueOrDefault(ID);
        }

        /// <summary>
        /// Tryes find identity by id
        /// </summary>
        public static bool TryFindByID(this uint ID, out NetworkIdentity identity)
        {
            return NetworkClient.spawned.TryGetValue(ID, out identity);
        }

        /// <summary>
        /// Finds identity by id
        /// </summary>
        public static async UniTask<NetworkIdentity> FindByIDAsync(this uint ID, CancellationToken token = default)
        {
            NetworkIdentity identity;

            while (!NetworkClient.spawned.TryGetValue(ID, out identity))
            {
                await UniTask.NextFrame(token);
            }

            return identity;
        }

        #endregion

        #region FindByUid TComponent

        /// <summary>
        /// Finds identity and component by id
        /// </summary>
        public static TComponent FindByID<TComponent>(this uint ID) where TComponent : Component
        {
            return FindByID(ID)?.GetComponent<TComponent>();
        }

        /// <summary>
        /// Tryes find identity and component by id
        /// </summary>
        public static bool TryFindByID<TComponent>(this uint ID, out TComponent component) where TComponent : Component
        {
            component = null;
            return TryFindByID(ID, out var identity) && identity.TryGetComponent(out component);
        }

        /// <summary>
        /// Finds identity and component by id
        /// </summary>
        public static async UniTask<TComponent> FindByIDAsync<TComponent>(this uint ID, CancellationToken token = default) where TComponent : Component
        {
            TComponent component;

            while (!NetworkClient.spawned.TryGetValue(ID, out var identity) || !identity.TryGetComponent<TComponent>(out component))
            {
                await UniTask.NextFrame(token);
            }

            return component;
        }

        #endregion



        #region Spawn

        /// <summary>
        /// Same as NetworkServer.Spawn
        /// </summary>
        public static T Spawn<T>(this T component, NetworkConnectionToClient ownerConnection = null) where T : Component
        {
            NetworkServer.Spawn(component.gameObject, ownerConnection);
            return component;
        }

        /// <summary>
        /// Same as NetworkServer.Spawn
        /// </summary>
        public static Component Spawn(this Component component, NetworkConnectionToClient ownerConnection = null)
        {
            NetworkServer.Spawn(component.gameObject, ownerConnection);
            return component;
        }

        /// <summary>
        /// Same as NetworkServer.Spawn
        /// </summary>
        public static GameObject Spawn(this GameObject obj, NetworkConnectionToClient ownerConnection = null)
        {
            NetworkServer.Spawn(obj, ownerConnection);
            return obj;
        }

        #endregion

        #region Write & Read

        // while SyncList<T> is recommended for NetworkBehaviours,
        // structs may have .List<T> members which weaver needs to be able to
        // fully serialize for NetworkMessages etc.
        // note that Weaver/Writers/GenerateWriter() handles this manually.
        public static void WriteReadonlyList<T>(this NetworkWriter writer, IReadOnlyList<T> list)
        {
            // we offset count by '1' to easily support null without writing another byte.
            // encoding null as '0' instead of '-1' also allows for better compression
            // (ushort vs. short / varuint vs. varint) etc.
            if (list is null)
            {
                // most sizes are small, write size as VarUInt!
                Compression.CompressVarUInt(writer, 0u);
                // writer.WriteUInt(0);
                return;
            }

            // check if within max size, otherwise Reader can't read it.
            if (list.Count > NetworkReader.AllocationLimit)
                throw new IndexOutOfRangeException($"NetworkWriter.WriteList - List<{typeof(T)}> too big: {list.Count} elements. Limit: {NetworkReader.AllocationLimit}");

            // most sizes are small, write size as VarUInt!
            Compression.CompressVarUInt(writer, checked((uint)list.Count) + 1u);
            // writer.WriteUInt(checked((uint)list.Count) + 1u);
            for (int i = 0; i < list.Count; i++)
                writer.Write(list[i]);
        }



        public static void WriteArray2D<T>(this NetworkWriter writer, Array2D<T> array)
        {
            writer.WriteReadonlyArray2D(array);
        }

        public static void WriteReadonlyArray2D<T>(this NetworkWriter writer, IReadOnlyArray2D<T> array)
        {
            // we offset count by '1' to easily support null without writing another byte.
            // encoding null as '0' instead of '-1' also allows for better compression
            // (ushort vs. short / varuint vs. varint) etc.
            if (array is null)
            {
                // most sizes are small, write size as VarUInt!
                Compression.CompressVarUInt(writer, 0u);
                Compression.CompressVarUInt(writer, 0u);
                // writer.WriteUInt(0);
                return;
            }

            // check if within max size, otherwise Reader can't read it.
            if (array.Count > NetworkReader.AllocationLimit)
                throw new IndexOutOfRangeException($"NetworkWriter.WriteList - List<{typeof(T)}> too big: {array.Count} elements. Limit: {NetworkReader.AllocationLimit}");

            // most sizes are small, write size as VarUInt!
            Compression.CompressVarUInt(writer, checked((uint)array.Width) + 1u);
            Compression.CompressVarUInt(writer, checked((uint)array.Height) + 1u);

            writer.WriteReadonlyList(array.RawData);
        }



        public static Array2D<T> ReadArray2D<T>(this NetworkReader reader)
        {
            // we offset count by '1' to easily support null without writing another byte.
            // encoding null as '0' instead of '-1' also allows for better compression
            // (ushort vs. short / varuint vs. varint) etc.

            // most sizes are small, read size as VarUInt!
            var width = (int)Compression.DecompressVarUInt(reader);
            var height = (int)Compression.DecompressVarUInt(reader);
            //uint length = reader.ReadUInt();
            if (width == 0 || height == 0) return null;
            width -= 1;
            height -= 1;

            // prevent allocation attacks with a reasonable limit.
            //   server shouldn't allocate too much on client devices.
            //   client shouldn't allocate too much on server in ClientToServer [SyncVar]s.
            if (width * height > NetworkReader.AllocationLimit)
            {
                // throw EndOfStream for consistency with ReadBlittable when out of data
                throw new EndOfStreamException($"NetworkReader attempted to allocate an Array<{typeof(T)}> with {width * height} elements, which is larger than the allowed limit of {NetworkReader.AllocationLimit}.");
            }

            // we can't check if reader.Remaining < length,
            // because we don't know sizeof(T) since it's a managed type.
            // if (length > reader.Remaining) throw new EndOfStreamException($"Received array that is too large: {length}");

            var data = reader.ReadArray<T>();
            return new Array2D<T>(data, width, height);
        }


        public static void WriteItem(this NetworkWriter writer, Item item)
        {
            writer.WriteString(JsonConvert.SerializeObject(item));
        }

        public static Item ReadItem(this NetworkReader reader)
        {
            return JsonConvert.DeserializeObject<Item>(reader.ReadString());
        }


        public static void WriteShape(this NetworkWriter writer, Array2D<bool> shape)
        {
            writer.WriteArray2D(shape);
        }

        public static Array2D<bool> ReadShape(this NetworkReader reader)
        {
            return reader.ReadArray2D<bool>();
        }

        #endregion

        #region Compress Vectors

        public static void WriteAndCompressVector2(NetworkWriter writer, Vector2 value, Vector2 precision)
        {
            Compression.ScaleToLong(value.x, precision.x, out long x);
            Compression.ScaleToLong(value.y, precision.y, out long y);
            Compression.CompressVarInt(writer, x);
            Compression.CompressVarInt(writer, y);
        }

        public static Vector2 ReadCompressedVector2(NetworkReader reader, Vector2 precision)
        {
            var x = Compression.DecompressVarInt(reader);
            var y = Compression.DecompressVarInt(reader);
            var x1 = Compression.ScaleToFloat(x, precision.x);
            var y1 = Compression.ScaleToFloat(y, precision.y);
            return new(x1, y1);
        }

        public static void WriteAndCompressVector3(NetworkWriter writer, Vector3 value, Vector3 precision)
        {
            Compression.ScaleToLong(value.x, precision.x, out long x);
            Compression.ScaleToLong(value.y, precision.y, out long y);
            Compression.ScaleToLong(value.z, precision.z, out long z);
            Compression.CompressVarInt(writer, x);
            Compression.CompressVarInt(writer, y);
            Compression.CompressVarInt(writer, z);
        }

        public static Vector3 ReadCompressedVector3(NetworkReader reader, Vector3 precision)
        {
            var x = Compression.DecompressVarInt(reader);
            var y = Compression.DecompressVarInt(reader);
            var z = Compression.DecompressVarInt(reader);
            var x1 = Compression.ScaleToFloat(x, precision.x);
            var y1 = Compression.ScaleToFloat(y, precision.y);
            var z1 = Compression.ScaleToFloat(z, precision.z);
            return new(x1, y1, z1);
        }

        #endregion
    }
}
