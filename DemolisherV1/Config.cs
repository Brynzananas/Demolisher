using BepInEx.Configuration;
using HG;
using RoR2;
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
            SharpnessConfig.Init();
            SoftnessConfig.Init();
            ChaosConfig.Init();
            BootsConfig.Init();
            MediumMeleeAttackConfig.Init();
            FireGrenadeConfig.Init();
            ShieldChargeConfig.Init();
            WhirlwindMeleeConfig.Init();
            ParryConfig.Init();
            CollapseConfig.Init();
            FireTallSwordConfig.Init();
            SlicingConfig.Init();
            ChainDashConfig.Init();
            FlyConfig.Init();
            LaserConfig.Init();
        }
    }
    public static class SharpnessConfig
    {
        public static void Init()
        {
            SharpnessCritAddition = CreateConfig(SharpnessName, "Crit Addition per Stack", 10f, "");
            SharpnessDamageMultiplier = CreateConfig(SharpnessName, "First Hit Damage Multiplier", 3f, "");
            SharpnessCooldown = CreateConfig(SharpnessName, "First Hit Cooldown", 10f, "");
        }
        public static ConfigEntry<float> SharpnessCritAddition;
        public static ConfigEntry<float> SharpnessDamageMultiplier;
        public static ConfigEntry<float> SharpnessCooldown;
    }
    public static class SoftnessConfig
    {
        public static void Init()
        {
            SoftnessHealOnHitPercentage = CreateConfig(SoftnessName, "Heal on Hit Percentage", 5f, "");
            SoftnessHealOnKillPercentage = CreateConfig(SoftnessName, "Heal on Kill Percentage", 15f, "");
        }
        public static ConfigEntry<float> SoftnessHealOnHitPercentage;
        public static ConfigEntry<float> SoftnessHealOnKillPercentage;
    }
    public static class ChaosConfig
    {
        public static void Init()
        {
            ChaosDamageCoefficient = CreateConfig(ChaosName, DamageCoefficientName, 5f, "");
            ChaosProcCoefficient = CreateConfig(ChaosName, ProcCoefficientName, 1f, "");
            ChaosRadius = CreateConfig(ChaosName, RadiusName, 12f, "");
            ChaosForce = CreateConfig(ChaosName, ForceName, 300f, "");
            ChaosCooldown = CreateConfig(ChaosName, "Cooldown", 10f, "");
        }
        public static ConfigEntry<float> ChaosDamageCoefficient;
        public static ConfigEntry<float> ChaosProcCoefficient;
        public static ConfigEntry<float> ChaosRadius;
        public static ConfigEntry<float> ChaosForce;
        public static ConfigEntry<float> ChaosCooldown;
    }
    public static class BootsConfig
    {
        public static void Init()
        {
            stompNeededVelocity = CreateConfig(BootsName, "Minimum Velocity to Stomp", 48f, "");
            stompBaseDamageCoefficient = CreateConfig(BootsName, DamageCoefficientName, 1f, "");
            stompVelocityDamageCoefficient = CreateConfig(BootsName, SpeedDamageCoefficientName, 0.2f, "");
            stompProcCoefficient = CreateConfig(BootsName, ProcCoefficientName, 1f, "");
            stompBaseRadius = CreateConfig(BootsName, RadiusName, 3f, "");
            stompVelocityRadiusMultiplier = CreateConfig(BootsName, SpeedRadiusName, 0.2f, "");
            stompForce = CreateConfig(BootsName, ForceName, 100f, "");
            stompFalloff = CreateConfig(BootsName, BlastFalloffName, BlastAttack.FalloffModel.Linear, "");
        }
        public static ConfigEntry<float> stompNeededVelocity;
        public static ConfigEntry<float> stompForce;
        public static ConfigEntry<float> stompBaseRadius;
        public static ConfigEntry<float> stompBaseDamageCoefficient;
        public static ConfigEntry<float> stompVelocityDamageCoefficient;
        public static ConfigEntry<float> stompVelocityRadiusMultiplier;
        public static ConfigEntry<BlastAttack.FalloffModel> stompFalloff;
        public static ConfigEntry<float> stompProcCoefficient;
    }
    public static class MediumMeleeAttackConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(MediumMeleeAttackName, DamageCoefficientName, 3f, "");
            procCoefficient = CreateConfig(MediumMeleeAttackName, ProcCoefficientName, 1f, "");
            baseDuration = CreateConfig(MediumMeleeAttackName, DurationName, 0.2f, "");
            baseAttackDuration = CreateConfig(MediumMeleeAttackName, AttackDurationName, 0.3f, "");
            radius = CreateConfig(MediumMeleeAttackName, RadiusName, 3f, "");
            maxDistance = CreateConfig(MediumMeleeAttackName, RangeName, 9f, "");
            force = CreateConfig(MediumMeleeAttackName, ForceName, 500f, "");
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
            damageCoefficient = CreateConfig(FireGrenadeName, DamageCoefficientName, 3f, "");
            baseDuration = CreateConfig(FireGrenadeName, DurationName, 0.5f, "");
            force = CreateConfig(FireGrenadeName, ForceName, 500f, "");
            maxCharge = CreateConfig(FireGrenadeName, MaxChargeName, 1f, "");
        }
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<float> maxCharge;
    }
    public static class ShieldChargeConfig
    {
        public static void Init()
        {
            baseDuration = CreateConfig(ShieldChargeName, DurationName, 1.5f, "");
            baseWalkSpeedMultiplier = CreateConfig(ShieldChargeName, SpeedMultiplierName, 3.5f, "");
            shieldBashDamageCoefficient = CreateConfig(ShieldChargeName, DamageCoefficientName, 2f, "");
            shieldBashSpeedDamageCoefficient = CreateConfig(ShieldChargeName, SpeedDamageCoefficientName, 1f, "");
            shieldBashProcCoefficient = CreateConfig(ShieldChargeName, ProcCoefficientName, 1f, "");
            shieldBashRadiusMultiplier = CreateConfig(ShieldChargeName, RadiusName, 4f, "");
            shieldBashDistance = CreateConfig(ShieldChargeName, RangeName, 9f, "");
        }
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<float> baseWalkSpeedMultiplier;
        public static ConfigEntry<float> shieldBashRadiusMultiplier;
        public static ConfigEntry<float> shieldBashDistance;
        public static ConfigEntry<float> shieldBashDamageCoefficient;
        public static ConfigEntry<float> shieldBashSpeedDamageCoefficient;
        public static ConfigEntry<float> shieldBashProcCoefficient;
        public static ConfigEntry<float> shieldBashTimer;
        public static ConfigEntry<float> shieldBashVelocityForceMultiplier;
        public static ConfigEntry<float> shieldBashGravityForceMultiplier;
    }
    public static class WhirlwindMeleeConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(WhirlwindMeleeName, DamageCoefficientName, 2f, "");
            procCoefficient = CreateConfig(WhirlwindMeleeName, ProcCoefficientName, 1f, "");
            maxDistance = CreateConfig(WhirlwindMeleeName, RangeName, 6f, "");
            force = CreateConfig(WhirlwindMeleeName, ForceName, 300f, "");
            radius = CreateConfig(WhirlwindMeleeName, RadiusName, 9f, "");
            baseDegreesPerSecond = CreateConfig(WhirlwindMeleeName, MovementControlName, 90f, "");
            baseRotationsPerSecond = CreateConfig(WhirlwindMeleeName, "Rotations per Second", 5f, "");
        }
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> maxDistance;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> baseDegreesPerSecond;
        public static ConfigEntry<float> baseRotationsPerSecond;
    }

    public static class ParryConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(ParryName, DamageCoefficientName, 2f, "");
            procCoefficient = CreateConfig(ParryName, ProcCoefficientName, 1f, "");
            baseParryWindow = CreateConfig(ParryName, "Parry Window", 1f, "");
            invincibilityTime = CreateConfig(ParryName, "Invincibility Time", 1f, "");
            force = CreateConfig(ParryName, ForceName, 300f, "");
            radius = CreateConfig(ParryName, RadiusName, 18f, "");
            baseMovementStart = CreateConfig(ParryName, "Start Movement Boost", 24f, "");
            baseMovementEnd = CreateConfig(ParryName, "Parry Knockback", 24f, "");
        }
        public static ConfigEntry<float> baseParryWindow;
        public static ConfigEntry<float> invincibilityTime;
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> baseMovementStart;
        public static ConfigEntry<float> baseMovementEnd;
    }
    public static class CollapseConfig
    {
        public static void Init()
        {
            bulletDamageCoefficient = CreateConfig(CollapseName, BulletName + " " + DamageCoefficientName, 15f, "");
            bulletProcCoefficient = CreateConfig(CollapseName, BulletName + " " + ProcCoefficientName, 1f, "");
            bulletForce = CreateConfig(CollapseName, BulletName + " " + ForceName, 1000f, "");
            bulletRadius = CreateConfig(CollapseName, BulletName + " " + RadiusName, 2f, "");
            explosionDamageCoefficient = CreateConfig(CollapseName, ExplosionName + " " + DamageCoefficientName, 15f, "");
            explosionProcCoefficient = CreateConfig(CollapseName, ExplosionName + " " + ProcCoefficientName, 15f, "");
            explosionForce = CreateConfig(CollapseName, ExplosionName + " " + ForceName, 1000f, "");
            explosionRadius = CreateConfig(CollapseName, ExplosionName + " " + RadiusName, 24f, "");
            selfForce = CreateConfig(CollapseName, "Self Push", 60f, "");
            selfForceGrounded = CreateConfig(CollapseName, "Self Push on Ground", 24f, "");
        }
        public static ConfigEntry<float> bulletDamageCoefficient;
        public static ConfigEntry<float> bulletProcCoefficient;
        public static ConfigEntry<float> explosionDamageCoefficient;
        public static ConfigEntry<float> explosionProcCoefficient;
        public static ConfigEntry<float> bulletForce;
        public static ConfigEntry<float> explosionForce;
        public static ConfigEntry<float> bulletRadius;
        public static ConfigEntry<float> explosionRadius;
        public static ConfigEntry<float> selfForce;
        public static ConfigEntry<float> selfForceGrounded;
    }
    public static class FireTallSwordConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(FireTallSwordName, DamageCoefficientName, 5f, "");
            baseDuration = CreateConfig(FireTallSwordName, DurationName, 0.5f, "");
            force = CreateConfig(FireTallSwordName, ForceName, 300f, "");
        }
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> force;
    }
    public static class SlicingConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(SlicingName, DamageCoefficientName, 5f, "");
            procCoefficient = CreateConfig(SlicingName, ProcCoefficientName, 1f, "");
            force = CreateConfig(SlicingName, ForceName, 300f, "");
            radius = CreateConfig(SlicingName, RadiusName, 3f, "");
            baseDistance = CreateConfig(SlicingName, DistanceName, 24f, "");
            baseDuration = CreateConfig(SlicingName, DurationName, 12f, "");
            baseTimeDivisionMultiplier = CreateConfig(SlicingName, "Time Reduction", 10f, "");
            stockMultiplier = CreateConfig(SlicingName, "Stock Multiplier", 4, "");
        }
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> baseTimeDivisionMultiplier;
        public static ConfigEntry<float> baseDistance;
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<int> stockMultiplier;
    }
    public static class ChainDashConfig
    {
        public static void Init()
        {
            baseStartWindow = CreateConfig(ChainDashName, "Start Window", 0.2f, "");
            baseEndWindow = CreateConfig(ChainDashName, "End Window", 0.4f, "");
            speedMultiplier = CreateConfig(ChainDashName, SpeedMultiplierName, 5f, "");
            moveVectorSmoothTime = CreateConfig(ChainDashName, "Speed Smooth Time", 0.5f, "");
        }
        public static ConfigEntry<float> baseStartWindow;
        public static ConfigEntry<float> baseEndWindow;
        public static ConfigEntry<float> speedMultiplier;
        public static ConfigEntry<float> moveVectorSmoothTime;
    }
    public static class FlyConfig
    {
        public static void Init()
        {
            baseFlyVectorSmoothTime = CreateConfig(FlyName, MovementControlName, 0.2f, "");
            baseSpeedMultiplier = CreateConfig(FlyName, SpeedMultiplierName, 1f, "");
            baseSpeedSmoothTime = CreateConfig(FlyName, "Speed Smooth Time", 1f, "");
            groundPush = CreateConfig(FlyName, "Vertical Push if Grounded", 5f, "");
            stompBaseDamageCoefficient = CreateConfig(FlyName, DamageCoefficientName, 2f, "");
            stompVelocityDamageCoefficient = CreateConfig(FlyName, SpeedDamageCoefficientName, 0.5f, "");
            stompProcCoefficient = CreateConfig(FlyName, ProcCoefficientName, 1f, "");
            stompBaseRadius = CreateConfig(FlyName, RadiusName, 3f, "");
            stompVelocityRadiusMultiplier = CreateConfig(FlyName, SpeedRadiusName, 0.2f, "");
            stompForce = CreateConfig(FlyName, ForceName, 100f, "");
            stompFalloff = CreateConfig(FlyName, BlastFalloffName, BlastAttack.FalloffModel.Linear, "");
        }
        public static ConfigEntry<float> baseFlyVectorSmoothTime;
        public static ConfigEntry<float> baseSpeedMultiplier;
        public static ConfigEntry<float> baseSpeedSmoothTime;
        public static ConfigEntry<float> groundPush;
        public static ConfigEntry<float> stompForce;
        public static ConfigEntry<float> stompBaseRadius;
        public static ConfigEntry<float> stompBaseDamageCoefficient;
        public static ConfigEntry<float> stompVelocityDamageCoefficient;
        public static ConfigEntry<float> stompVelocityRadiusMultiplier;
        public static ConfigEntry<BlastAttack.FalloffModel>  stompFalloff;
        public static ConfigEntry<float> stompProcCoefficient;
    }
    public static class LaserConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(LaserName, DamageCoefficientName, 0.5f, "");
            procCoefficient = CreateConfig(LaserName, ProcCoefficientName, 1f, "");
            hitInterval = CreateConfig(LaserName, "Hit Interval", 0.1f, "");
            force = CreateConfig(LaserName, ForceName, 0f, "");
            range = CreateConfig(LaserName, RangeName, 128f, "");
            radius = CreateConfig(LaserName, RadiusName, 3f, "");
        }
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> range;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> hitInterval;
    }
}
