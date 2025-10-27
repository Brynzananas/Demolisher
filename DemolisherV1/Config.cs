using BepInEx.Configuration;
using HG;
using System;
using System.Collections.Generic;
using System.Text;
using static Demolisher.Keywords;
using static Demolisher.Utils;

namespace Demolisher
{
    public static class Config
    {
        public static void Init()
        {
            MediumMeleeAttackConfig.Init();
            FireGrenadeConfig.Init();
        }
    }
    public static class MediumMeleeAttackConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(MediumMeleeAttackName, DamageCoefficientName, 2f, "");
            procCoefficient = CreateConfig(MediumMeleeAttackName, ProcCoefficientName, 1f, "");
            baseDuration = CreateConfig(MediumMeleeAttackName, DurationName, 0.2f, "");
            baseAttackDuration = CreateConfig(MediumMeleeAttackName, AttackDurationName, 0.6f, "");
            radius = CreateConfig(MediumMeleeAttackName, RadiusName, 3f, "");
            maxDistance = CreateConfig(MediumMeleeAttackName, DistanceName, 9f, "");
            force = CreateConfig(MediumMeleeAttackName, ForceName, 300f, "");
        }
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> baseAttackDuration;
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> maxDistance;
    }
    public static class FireGrenadeConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(FireGrenadeName, DamageCoefficientName, 2f, "");
            baseDuration = CreateConfig(FireGrenadeName, DurationName, 0.5f, "");
            force = CreateConfig(FireGrenadeName, ForceName, 300f, "");
            maxCharge = CreateConfig(FireGrenadeName, MaxChargeName, 1f, "");
        }
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<float> maxCharge;
    }
    public static class SHieldCharge
    {
        //public static void Init()
        //{
        //    baseDuration = CreateConfig(FireGrenadeName, DurationName, 0.5f, "");
        //}
        //public static ConfigEntry<float> baseDuration = 1.5f;
        //public static ConfigEntry<float> baseWalkSpeedMultiplier = 3.5f;
        //public static ConfigEntry<float> shieldBashRadiusMultiplier = 3f;
        //public static ConfigEntry<float> shieldBashDamageCoefficient = 2f;
        //public static ConfigEntry<float> shieldBashSpeedDamageCoefficient = 0.5f;
        //public static ConfigEntry<float> shieldBashProcCoefficient = 1f;
        //public static ConfigEntry<float> shieldBashBaseForce = 200f;
        //public static ConfigEntry<float> shieldBashVelocityForce = 100f;
        //public static ConfigEntry<float> shieldBashTimer = 1f;
        //public static ConfigEntry<float> shieldBashVelocityForceMultiplier = 1f;
        //public static ConfigEntry<float> shieldBashGravityForceMultiplier = 1f;
    }
}
