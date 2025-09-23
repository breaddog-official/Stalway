using Breaddog.AssetsManagement;
using UnityEngine;

namespace Breaddog.Gameplay.StorageManagement
{
    [CreateAssetMenu(fileName = "ItemBasicContainer", menuName = "Stalway/Containers/Item Basic")]
    public class ItemBasicContainer : PolymorphicContainerSO<ItemBasic, Item> { }
}