using Breaddog.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Breaddog.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Health")]
        public TMP_Text HealthText;
        public TMP_Text ArmorText;
        public Image ArmorTierImage;
        public Sprite[] ArmorTierSprites;


        [Header("Weapons")]
        //public TMP_Text AmmoText;
        //public TMP_Text AmmoBackpackText;
        public Image[] WeaponsImages;


        public Entity ObservingEntity => Entity.ObservingEntity;



        protected void Update()
        {
            if (ObservingEntity == null)
                return;


            if (ObservingEntity.TryFindAbillity(out AbillityHealth health))
            {
                // Это конечно плохо что я прописываю дизайн прям в скрипте, но я думаю это не то о чём мне стоит волноваться
                SetText(HealthText, $"{health.Health}");
                //SetText(ArmorText, $"{health.Armor}%");
                //SetSprite(ArmorTierImage, ArmorTierSprites.GetInRange(health.ArmorTier));
            }

            if (ObservingEntity.TryFindAbillity(out AbillityInventory inventory))
            {
                //for (int i = 0; i < inventory.Items.Count; i++)
                //{
                //    SetSprite(WeaponsImages[i], inventory.Items[i]?.InventorySprite);
                //}
            }
        }

        private void SetText(TMP_Text text, string value)
        {
            if (text != null)
                text.SetText(value);
        }

        private void SetSprite(Image image, Sprite value)
        {
            if (image != null)
                image.sprite = value;
        }
    }
}