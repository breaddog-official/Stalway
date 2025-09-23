using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using Breaddog.Extensions;

namespace Breaddog.SaveManagement
{
    public enum DataLocation
    {
        [Tooltip("������ ����� ��������������� ��������� ����� � �����")]
        PreferDefault,
        [Tooltip("������ ����� ��������������� ��������� � �����, ������� �� ��������� ��� �������� ����")]
        PreferPersistent
    }

    public static class SaveManager
    {
        #region Constants

        /// <summary>
        /// Persistent path for player's data
        /// </summary>

        // Application.productName needed for platforms like UWP, because their persistentDataPath only has a company in path

        public static string PlayerDataPath => Path.Combine(GetDataPath(DataLocation.PreferPersistent), $"{Application.productName}PlayerData");

        /// <summary>
        /// Path for configs that are updated with the game
        /// </summary>
        public static string ConfigsPath => Path.Combine(GetDataPath(DataLocation.PreferDefault), "Configs");


        public static bool SupportIO => Application.platform.SupportDataPath() || Application.platform.SupportPersistentDataPath();

        #endregion


        #region TrySave

        public static bool TrySave(in string value, string path, IStringSaver saver)
        {
            try
            {
                saver.Save(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> TrySaveAsync(string value, string path, IStringSaver saver)
        {
            try
            {
                await saver.SaveAsync(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region TryLoad

        public static bool TryLoad(string path, IStringSaver saver, out string value)
        {
            try
            {
                value = saver.Load(path);
                return value != null;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);

                value = null;
                return false;
            }
        }

        public static async UniTask<(bool, string)> TryLoadAsync(string path, IStringSaver saver)
        {
            string value;
            try
            {
                value = await saver.LoadAsync(path);
                return (value != null, value);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);

                value = null;
                return (false, value);
            }
        }

        #endregion

        #region TrySaveBytes

        public static bool TrySaveBytes(in byte[] value, string path, IBytesSaver saver)
        {
            try
            {
                saver.SaveBytes(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> TrySaveBytesAsync(byte[] value, string path, IBytesSaver saver)
        {
            try
            {
                await saver.SaveBytesAsync(path, value);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region TryLoadBytes

        public static bool TryLoadBytes(string path, IBytesSaver saver, out byte[] value)
        {
            try
            {
                value = saver.LoadBytes(path);
                return value != null;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);

                value = null;
                return false;
            }
        }

        public static async UniTask<(bool, byte[])> TryLoadBytesAsync(string path, IBytesSaver saver)
        {
            byte[] value;
            try
            {
                value = await saver.LoadBytesAsync(path);
                return (value != null, value);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);

                value = null;
                return (false, value);
            }
        }

        #endregion


        #region GetDataPath

        public static string GetDataPath(DataLocation preferLocation = DataLocation.PreferDefault)
        {
            var platform = Application.platform;

            var dataPath = platform.SupportDataPath() ? Application.dataPath : string.Empty;
            var persistentPath = platform.SupportPersistentDataPath() ? Application.persistentDataPath : string.Empty;

            return preferLocation switch
            {
                DataLocation.PreferPersistent => persistentPath ?? dataPath,
                _ => dataPath ?? persistentPath
            };
        }

        #endregion


        #region SerializeAndSave

        public static bool SerializeAndSave(object value, string path, IStringSaver saver, IStringSerializer serializer)
        {
            try
            {
                string serialized = serializer.Serialize(value);
                return TrySave(serialized, path, saver);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SerializeAndSaveAsync(object value, string path, IStringSaver saver, IStringSerializer serializer)
        {
            try
            {
                string serialized = await serializer.SerializeAsync(value);
                return await TrySaveAsync(serialized, path, saver);
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region LoadAndDeserialize

        public static object LoadAndDeserialize(string path, IStringSaver saver, IStringSerializer serializer)
        {
            string loaded = saver.Load(path);
            return serializer.Deserialize(loaded);
        }

        public static async UniTask<object> LoadAndDeserializeAsync(string path, IStringSaver saver, IStringSerializer serializer)
        {
            string loaded = await saver.LoadAsync(path);
            return await serializer.DeserializeAsync(loaded);
        }

        #endregion

        #region LoadAndDeserialize<T>

        public static T LoadAndDeserialize<T>(string path, IStringSaver saver, IStringSerializer serializer)
        {
            string loaded = saver.Load(path);
            return serializer.DeserializeType<T>(loaded);
        }

        public static async UniTask<T> LoadAndDeserializeAsync<T>(string path, IStringSaver saver, IStringSerializer serializer)
        {
            string loaded = await saver.LoadAsync(path);
            return await serializer.DeserializeTypeAsync<T>(loaded);
        }

        #endregion

        #region TryLoadAndDeserialize

        public static bool TryLoadAndDeserialize(string path, IStringSaver saver, IStringSerializer serializer, out object value)
        {
            value = default;

            try
            {
                string loaded = saver.Load(path);
                value = serializer.Deserialize(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static bool TryLoadAndDeserialize<T>(string path, IStringSaver saver, IStringSerializer serializer, out T value)
        {
            value = default;

            try
            {
                string loaded = saver.Load(path);
                value = serializer.DeserializeType<T>(loaded);

                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion


        #region SerializeAndSaveBytes

        public static bool SerializeAndSaveBytes(object value, string path, IBytesSaver saver, IBytesSerializer serializer)
        {
            try
            {
                byte[] serialized = serializer.SerializeBytes(value);
                saver.SaveBytes(path, serialized);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static async UniTask<bool> SerializeAndSaveAsyncBytes(object value, string path, IBytesSaver saver, IBytesSerializer serializer)
        {
            try
            {
                byte[] serialized = await serializer.SerializeBytesAsync(value);
                await saver.SaveBytesAsync(path, serialized);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

        #region LoadAndDeserializeBytes

        public static object LoadAndDeserializeBytes(string path, IBytesSaver saver, IBytesSerializer serializer)
        {
            byte[] loaded = saver.LoadBytes(path);
            return serializer.DeserializeBytes(loaded);
        }

        public static async UniTask<object> LoadAndDeserializeAsyncBytes(string path, IBytesSaver saver, IBytesSerializer serializer)
        {
            byte[] loaded = await saver.LoadBytesAsync(path);
            return await serializer.DeserializeBytesAsync(loaded);
        }

        #endregion

        #region LoadAndDeserializeBytes<T>

        public static T LoadAndDeserializeBytes<T>(string path, IBytesSaver saver, IBytesSerializer serializer)
        {
            byte[] loaded = saver.LoadBytes(path);
            return serializer.DeserializeBytesType<T>(loaded);
        }

        public static async UniTask<T> LoadAndDeserializeAsyncBytes<T>(string path, IBytesSaver saver, IBytesSerializer serializer)
        {
            byte[] loaded = await saver.LoadBytesAsync(path);
            return await serializer.DeserializeBytesTypeAsync<T>(loaded);
        }

        #endregion

        #region TryLoadAndDeserializeBytes

        public static bool TryLoadAndDeserializeBytes(string path, IBytesSaver saver, IBytesSerializer serializer, out object value)
        {
            value = default;
            try
            {
                byte[] loaded = saver.LoadBytes(path);
                value = serializer.DeserializeBytes(loaded);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        public static bool TryLoadAndDeserializeBytes<T>(string path, IBytesSaver saver, IBytesSerializer serializer, out T value)
        {
            value = default;
            try
            {
                byte[] loaded = saver.LoadBytes(path);
                value = serializer.DeserializeBytesType<T>(loaded);
                return true;
            }
            catch (Exception exp)
            {
                Debug.LogException(exp);
                return false;
            }
        }

        #endregion

    }
}
