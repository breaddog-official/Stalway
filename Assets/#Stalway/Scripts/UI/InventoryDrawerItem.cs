using Breaddog.AssetsManagement;
using Breaddog.Gameplay.StorageManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Breaddog.UI
{
    public class InventoryDrawerItem : MonoBehaviour
    {
        public RectTransform Rect;
        public Image Image;
        public TMP_Text TechNameText;
        public TMP_Text NameText;
        public TMP_Text DescriptionText;
        public TMP_Text WeightText;
        public TMP_Text MaxStackText;

        public StoredItem CurrentItem { get; private set; }
        private Asset<Sprite>? spriteAsset;
        private bool async;


        public virtual void Initialize(bool async)
        {
            this.async = async;
            Rect = GetComponent<RectTransform>();
        }

        public virtual async UniTask UpdateItem(StoredItem item, CancellationToken token = default)
        {
            CurrentItem = item;
            if (Image != null && (spriteAsset == null || spriteAsset.Value.path != CurrentItem.itemAsset.InventorySprite))
            {
                spriteAsset?.Dispose();
                if (async)
                    spriteAsset = await AssetsManager.GetAssetAsync<Sprite>(CurrentItem.itemAsset.InventorySprite, token: token);
                else
                    spriteAsset = AssetsManager.GetAsset<Sprite>(CurrentItem.itemAsset.InventorySprite);

                Image.sprite = spriteAsset.Value.asset;
            }

            TechNameText?.SetText(CurrentItem.itemAsset.Name);
            //nameText?.SetText(CurrentItem.NameTranslateKey);
            //descriptionText?.SetText(CurrentItem.DescriptionTranslateKey);
            WeightText?.SetText(CurrentItem.itemAsset.Weight.ToString());
            MaxStackText?.SetText(CurrentItem.itemAsset.MaxStack.ToString());
        }

        private void OnDisable()
        {
            if (spriteAsset != null)
            {
                Image.sprite = null;
                spriteAsset.Value.Dispose();
                spriteAsset = null;
            }
        }
    }
}
