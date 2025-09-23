using Breaddog.AssetsManagement;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [CreateAssetMenu(fileName = "ItemAssetLoader", menuName = "Stalway/Asset Loaders/Item")]
    public class AssetLoaderItem : AssetLoaderText<Item> { }
}