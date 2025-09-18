using Breaddog.AssetsManagement;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [CreateAssetMenu(fileName = "ItemDedicatedLoader", menuName = "Stalway/Dedicated Loaders/Item")]
    public class AssetLoaderItem : AssetLoaderText<Item> { }
}