using UnityEngine;

namespace Breaddog.Gameplay
{
    public interface IDamageReciever
    {
        public void TakeDamage(float damage, float armorDamage);
        public bool HasArmor();
    }
}