using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.Serialization;
using Mirror;
using Breaddog.Extensions;
using Sirenix.OdinInspector;

namespace Breaddog.Gameplay
{
    public class AbillityHealth : Abillity, IDamageReciever
    {
        [field: SyncVar]
        [Header("Health"), ProgressBar("MinHealth", "MaxHealth", ColorGetter = nameof(GetHealthBarColor))]
        [OdinSerialize] public float Health { get; private set; } = 100f;
        [OdinSerialize] public Vector2 HealthRange { get; private set; } = new(0f, 100f);


        public float MinHealth => HealthRange.x;
        public float MaxHealth => HealthRange.y;



        public override void Init()
        {

        }


        public void TakeDamage(float damage, float armorDamage = 0f)
        {
            Health = Mathf.Clamp(MinHealth, MaxHealth, Health - damage);
        }

        public bool HasArmor()
        {
            return false;
        }

        private Color GetHealthBarColor(float value) => value.GetProgressBarColor(MinHealth, MaxHealth, Color.red, Color.green);
    }
}
