using Breaddog.AssetsManagement;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [CreateAssetMenu(fileName = "ItemContainer", menuName = "Stalway/Containers/Item")]
    public class ItemContainer : ContainerSO<Item> { }
}