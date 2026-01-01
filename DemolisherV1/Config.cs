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
            RocketjumpingConfig.Init();
            VisualsConfig.Init();
            CreateConfigsForBulletAttackWeapon(Assets.SharpnessWeapon, SharpnessName, Language.InitSharpness);
            CreateConfigsForBulletAttackWeapon(Assets.SoftnessWeapon, SoftnessName, Language.InitSoftness);
            //CreateConfigsForBulletAttackWeapon(Assets.ChaosWeapon, ChaosName, Language.InitChaos);
            CreateConfigsForProjectileWeapon(Assets.StickyWeapon, StickyTrapLauncherName, Language.InitStickyLauncher);
            CreateConfigsForProjectileWeapon(Assets.GrenadeWeapon, GreandeLauncherName, Language.InitGrenadeLauncher);
            CreateConfigsForProjectileWeapon(Assets.BombWeapon, BombLauncherName, Language.InitBombLauncher);
            SharpnessConfig.Init();
            SoftnessConfig.Init();
            //ChaosConfig.Init();
            BootsConfig.Init();
            MediumMeleeAttackConfig.Init();
            FireGrenadeConfig.Init();
            ShieldChargeConfig.Init();
            WhirlwindMeleeConfig.Init();
            //ParryConfig.Init();
            CollapseConfig.Init();
            FireTallSwordConfig.Init();
            //SlicingConfig.Init();
            ChainDashConfig.Init();
            FlyConfig.Init();
            LaserConfig.Init();
        }
    }
    public static class RocketjumpingConfig
    {
        public const string name = "Rocketjumping";
        public static void Init()
        {
            RocketJumpForceMovespeedClamp = CreateConfig(name, "Rocketjump movespeed clamp", 1.5f, "");
            RocketJumpForceMovespeedDivide = CreateConfig(name, "Rocketjump movespeed bonus division", 100f, "");
            RocketJumpForceJumpClamp = CreateConfig(name, "Rocketjump jump clamp", 1.5f, "");
            RocketJumpForceJumpDivide = CreateConfig(name, "Rocketjump jump bonus division", 100f, "");
            RocketJumpAirControl = CreateConfig(name, "Rocketjump air control", 2f, "");
        }
        public static ConfigEntry<float> RocketJumpForceMovespeedClamp;
        public static ConfigEntry<float> RocketJumpForceMovespeedDivide;
        public static ConfigEntry<float> RocketJumpForceJumpClamp;
        public static ConfigEntry<float> RocketJumpForceJumpDivide;
        public static ConfigEntry<float> RocketJumpAirControl;
    }
    public static class VisualsConfig
    {
        public const string name = "Visuals";
        public static void Init()
        {
            Aura = CreateConfig(name, "Devil Mesh Trail", true, "");
            FeetSmoke = CreateConfig(name, "Rocketjump Feet Smoke", true, "");
            ExplosionLight = CreateConfig(name, "Explosion Light", true, "");
            ExplosionShake = CreateConfig(name, "Explosion Shake", true, "");
            LobbyPillar = CreateConfig(name, "Demolisher Lobby Pillar", true, "");
            LobbyRed = CreateConfig(name, "Demolisher Lobby Red Ambient", true, "");
            CrosshairRangedPrimaryCounter = CreateConfig(name, "Demolisher Crosshair Ranged Primary Counter", true, "");
            CrosshairRangedPrimaryCharge = CreateConfig(name, "Demolisher Crosshair Ranged Primary Charge Meter", true, "");
            CrosshairRangedSecondaryCounter = CreateConfig(name, "Demolisher Crosshair Ranged Secondary Counter", true, "");
            CrosshairRangedSecondaryCharge = CreateConfig(name, "Demolisher Crosshair Ranged Secondary Charge Meter", true, "");
            CrosshairMeleeSpecialCharge = CreateConfig(name, "Demolisher Crosshair Melee Special Charge Meter", true, "");
            CrosshairRangedSpecialCharge = CreateConfig(name, "Demolisher Crosshair Ranged Special Charge Meter", true, "");
            DemolisherVoicelines = CreateConfig(name, "Demolisher Voicelines", true, "");
            StickyRangeIndicator = CreateConfig(name, "Sticky Trap Blast Radius Indicator", true, "");
            //DemolisherBooleanConfigComponent[] explosionConfigs = Assets.Explosion.prefab.GetComponents<DemolisherBooleanConfigComponent>();
            //for (int i = 0; i < explosionConfigs.Length; i++)
            //{
            //    DemolisherBooleanConfigComponent demolisherBooleanConfigComponent = explosionConfigs[i];
            //    demolisherBooleanConfigComponent.config = i == 0 ? ExplosionLight : ExplosionShake;
            //}
            //DemolisherBooleanConfigComponent feetSmokeConfig = Assets.FeetEffect.GetComponent<DemolisherBooleanConfigComponent>();
            //feetSmokeConfig.config = FeetSmoke;
        }
        public static ConfigEntry<bool> Aura;
        public static ConfigEntry<bool> FeetSmoke;
        public static ConfigEntry<bool> ExplosionLight;
        public static ConfigEntry<bool> ExplosionShake;
        public static ConfigEntry<bool> LobbyPillar;
        public static ConfigEntry<bool> LobbyRed;
        public static ConfigEntry<bool> CrosshairRangedPrimaryCounter;
        public static ConfigEntry<bool> CrosshairRangedPrimaryCharge;
        public static ConfigEntry<bool> CrosshairRangedSecondaryCounter;
        public static ConfigEntry<bool> CrosshairRangedSecondaryCharge;
        public static ConfigEntry<bool> CrosshairMeleeSpecialCharge;
        public static ConfigEntry<bool> CrosshairRangedSpecialCharge;
        public static ConfigEntry<bool> DemolisherVoicelines;
        public static ConfigEntry<bool> StickyRangeIndicator;
    }
    public static class SharpnessConfig
    {
        public static void Init()
        {
            SharpnessCritAddition = CreateConfig(SharpnessName, "Crit Addition per Stack", 10f, "");
            SharpnessDamageMultiplier = CreateConfig(SharpnessName, "First Hit Damage Multiplier", 2f, "");
            SharpnessCooldown = CreateConfig(SharpnessName, "First Hit Cooldown", 10f, "");
            SharpnessCritAddition.SettingChanged += OnConfigChanged;
            SharpnessDamageMultiplier.SettingChanged += OnConfigChanged;
            SharpnessCooldown.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitSharpness();

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
            SoftnessHealOnHitPercentage.SettingChanged += OnConfigChanged;
            SoftnessHealOnKillPercentage.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitSoftness();
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
            ChaosDamageCoefficient.SettingChanged += OnConfigChanged;
            ChaosProcCoefficient.SettingChanged += OnConfigChanged;
            ChaosRadius.SettingChanged += OnConfigChanged;
            ChaosForce.SettingChanged += OnConfigChanged;
            ChaosCooldown.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitChaos();
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
            stompNeededVelocity = CreateConfig(BootsName, "Minimum Velocity to Stomp", 80f, "");
            stompBaseDamageCoefficient = CreateConfig(BootsName, DamageCoefficientName, 1f, "");
            stompVelocityDamageCoefficient = CreateConfig(BootsName, SpeedDamageCoefficientName, 0.1f, "");
            stompProcCoefficient = CreateConfig(BootsName, ProcCoefficientName, 1f, "");
            stompBaseRadius = CreateConfig(BootsName, RadiusName, 3f, "");
            stompVelocityRadiusMultiplier = CreateConfig(BootsName, SpeedRadiusName, 0.1f, "");
            stompForce = CreateConfig(BootsName, ForceName, 100f, "");
            stompFalloff = CreateConfig(BootsName, BlastFalloffName, BlastAttack.FalloffModel.Linear, "");
            pullStrength = CreateConfig(BootsName, "Pull Strength", 64f, "");
            stompNeededVelocity.SettingChanged += OnConfigChanged;
            stompBaseDamageCoefficient.SettingChanged += OnConfigChanged;
            stompVelocityDamageCoefficient.SettingChanged += OnConfigChanged;
            stompProcCoefficient.SettingChanged += OnConfigChanged;
            stompBaseRadius.SettingChanged += OnConfigChanged;
            stompVelocityRadiusMultiplier.SettingChanged += OnConfigChanged;
            stompForce.SettingChanged += OnConfigChanged;
            stompFalloff.SettingChanged += OnConfigChanged;
            pullStrength.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitBoots();
        public static ConfigEntry<float> stompNeededVelocity;
        public static ConfigEntry<float> stompForce;
        public static ConfigEntry<float> stompBaseRadius;
        public static ConfigEntry<float> stompBaseDamageCoefficient;
        public static ConfigEntry<float> stompVelocityDamageCoefficient;
        public static ConfigEntry<float> stompVelocityRadiusMultiplier;
        public static ConfigEntry<BlastAttack.FalloffModel> stompFalloff;
        public static ConfigEntry<float> stompProcCoefficient;
        public static ConfigEntry<float> pullStrength;
    }
    public static class MediumMeleeAttackConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(MediumMeleeAttackName, DamageCoefficientName, 3f, "");
            procCoefficient = CreateConfig(MediumMeleeAttackName, ProcCoefficientName, 1f, "");
            effectCoefficient = CreateConfig(MediumMeleeAttackName, EffectCoefficientName, 1f, "");
            baseDuration = CreateConfig(MediumMeleeAttackName, DurationName, 0.2f, "");
            baseAttackDuration = CreateConfig(MediumMeleeAttackName, AttackDurationName, 0.3f, "");
            radius = CreateConfig(MediumMeleeAttackName, RadiusName, 3f, "");
            maxDistance = CreateConfig(MediumMeleeAttackName, RangeName, 12f, "");
            hitJump = CreateConfig(MediumMeleeAttackName, "Vertical Velocity on Hit", 0.35f, "");
            force = CreateConfig(MediumMeleeAttackName, ForceName, 500f, "");
            damageCoefficient.SettingChanged += OnConfigChanged;
            procCoefficient.SettingChanged += OnConfigChanged;
            effectCoefficient.SettingChanged += OnConfigChanged;
            baseDuration.SettingChanged += OnConfigChanged;
            baseAttackDuration.SettingChanged += OnConfigChanged;
            radius.SettingChanged += OnConfigChanged;
            maxDistance.SettingChanged += OnConfigChanged;
            hitJump.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitMediumMelee();
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> effectCoefficient;
        public static ConfigEntry<float> baseAttackDuration;
        public static ConfigEntry<float> baseDuration;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> maxDistance;
        public static ConfigEntry<float> hitJump;
    }
    public static class FireGrenadeConfig
    {
        public static void Init()
        {
            damageCoefficient = CreateConfig(FireGrenadeName, DamageCoefficientName, 3f, "");
            baseDuration = CreateConfig(FireGrenadeName, DurationName, 0.5f, "");
            force = CreateConfig(FireGrenadeName, ForceName, 500f, "");
            maxCharge = CreateConfig(FireGrenadeName, MaxChargeName, 1f, "");
            damageCoefficient.SettingChanged += OnConfigChanged;
            baseDuration.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
            maxCharge.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e)
        {
            Language.InitGrenadeLauncher();
            Language.InitHookLauncher();
            Language.InitStickyLauncher();
            Language.InitDemolisherLauncher();
            Language.InitBombLauncher();
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
            baseDuration = CreateConfig(ShieldChargeName, DurationName, 2f, "");
            baseWalkSpeedMultiplier = CreateConfig(ShieldChargeName, SpeedMultiplierName, 3.5f, "");
            shieldBashDamageCoefficient = CreateConfig(ShieldChargeName, DamageCoefficientName, 2f, "");
            shieldBashSpeedDamageCoefficient = CreateConfig(ShieldChargeName, SpeedDamageCoefficientName, 1f, "");
            shieldBashProcCoefficient = CreateConfig(ShieldChargeName, ProcCoefficientName, 1f, "");
            shieldBashRadiusMultiplier = CreateConfig(ShieldChargeName, RadiusName, 4f, "");
            shieldBashDistance = CreateConfig(ShieldChargeName, RangeName, 9f, "");
            baseDuration.SettingChanged += OnConfigChanged;
            baseWalkSpeedMultiplier.SettingChanged += OnConfigChanged;
            shieldBashDamageCoefficient.SettingChanged += OnConfigChanged;
            shieldBashSpeedDamageCoefficient.SettingChanged += OnConfigChanged;
            shieldBashProcCoefficient.SettingChanged += OnConfigChanged;
            shieldBashRadiusMultiplier.SettingChanged += OnConfigChanged;
            shieldBashDistance.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitShieldBash();
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
            damageCoefficient = CreateConfig(WhirlwindMeleeName, DamageCoefficientName, 3f, "");
            procCoefficient = CreateConfig(WhirlwindMeleeName, ProcCoefficientName, 1f, "");
            maxDistance = CreateConfig(WhirlwindMeleeName, RangeName, 6f, "");
            force = CreateConfig(WhirlwindMeleeName, ForceName, 300f, "");
            radius = CreateConfig(WhirlwindMeleeName, RadiusName, 9f, "");
            baseDegreesPerSecond = CreateConfig(WhirlwindMeleeName, MovementControlName, 90f, "");
            baseRotationsPerSecond = CreateConfig(WhirlwindMeleeName, "Rotations per Second", 5f, "");
            damageCoefficient.SettingChanged += OnConfigChanged;
            procCoefficient.SettingChanged += OnConfigChanged;
            maxDistance.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
            radius.SettingChanged += OnConfigChanged;
            baseDegreesPerSecond.SettingChanged += OnConfigChanged;
            baseRotationsPerSecond.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitWhirlwind();
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
            damageCoefficient.SettingChanged += OnConfigChanged;
            procCoefficient.SettingChanged += OnConfigChanged;
            baseParryWindow.SettingChanged += OnConfigChanged;
            invincibilityTime.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
            baseMovementStart.SettingChanged += OnConfigChanged;
            baseMovementEnd.SettingChanged += OnConfigChanged;
            radius.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitParry();
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
            bulletDamageCoefficient = CreateConfig(CollapseName, BulletName + " " + DamageCoefficientName, 0f, "");
            bulletProcCoefficient = CreateConfig(CollapseName, BulletName + " " + ProcCoefficientName, 0f, "");
            bulletForce = CreateConfig(CollapseName, BulletName + " " + ForceName, 0f, "");
            bulletRadius = CreateConfig(CollapseName, BulletName + " " + RadiusName, 2f, "");
            explosionDamageCoefficient = CreateConfig(CollapseName, ExplosionName + " " + DamageCoefficientName, 15f, "");
            explosionProcCoefficient = CreateConfig(CollapseName, ExplosionName + " " + ProcCoefficientName, 1f, "");
            explosionForce = CreateConfig(CollapseName, ExplosionName + " " + ForceName, 1000f, "");
            explosionRadius = CreateConfig(CollapseName, ExplosionName + " " + RadiusName, 24f, "");
            selfForce = CreateConfig(CollapseName, "Self Push", 60f, "");
            selfForceGrounded = CreateConfig(CollapseName, "Self Push on Ground", 24f, "");
            bulletDamageCoefficient.SettingChanged += OnConfigChanged;
            bulletProcCoefficient.SettingChanged += OnConfigChanged;
            bulletForce.SettingChanged += OnConfigChanged;
            bulletRadius.SettingChanged += OnConfigChanged;
            explosionDamageCoefficient.SettingChanged += OnConfigChanged;
            explosionProcCoefficient.SettingChanged += OnConfigChanged;
            explosionForce.SettingChanged += OnConfigChanged;
            selfForce.SettingChanged += OnConfigChanged;
            selfForceGrounded.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitCollapse();
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
            damageCoefficient.SettingChanged += OnConfigChanged;
            baseDuration.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitSwordPillar();
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
            effectCoefficient = CreateConfig(SlicingName, EffectCoefficientName, 2f, "");
            force = CreateConfig(SlicingName, ForceName, 300f, "");
            radius = CreateConfig(SlicingName, RadiusName, 3f, "");
            baseDistance = CreateConfig(SlicingName, DistanceName, 24f, "");
            baseDuration = CreateConfig(SlicingName, DurationName, 12f, "");
            baseTimeDivisionMultiplier = CreateConfig(SlicingName, "Time Reduction", 10f, "");
            stockMultiplier = CreateConfig(SlicingName, "Stock Multiplier", 4, "");
            damageCoefficient.SettingChanged += OnConfigChanged;
            procCoefficient.SettingChanged += OnConfigChanged;
            radius.SettingChanged += OnConfigChanged;
            baseDistance.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
            baseDuration.SettingChanged += OnConfigChanged;
            baseTimeDivisionMultiplier.SettingChanged += OnConfigChanged;
            stockMultiplier.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitSlicing();
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> effectCoefficient;
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
            baseStartWindow.SettingChanged += OnConfigChanged;
            baseEndWindow.SettingChanged += OnConfigChanged;
            speedMultiplier.SettingChanged += OnConfigChanged;
            moveVectorSmoothTime.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitChainDash();
        public static ConfigEntry<float> baseStartWindow;
        public static ConfigEntry<float> baseEndWindow;
        public static ConfigEntry<float> speedMultiplier;
        public static ConfigEntry<float> moveVectorSmoothTime;
    }
    public static class FlyConfig
    {
        public static void Init()
        {
            baseFlyVectorSmoothTime = CreateConfig(FlyName, MovementControlName, 0.1f, "");
            baseSpeedMultiplier = CreateConfig(FlyName, SpeedMultiplierName, 1f, "");
            baseSpeedSmoothTime = CreateConfig(FlyName, "Speed Smooth Time", 1f, "");
            groundPush = CreateConfig(FlyName, "Vertical Push if Grounded", 5f, "");
            stompBaseDamageCoefficient = CreateConfig(FlyName, DamageCoefficientName, 10f, "");
            stompVelocityDamageCoefficient = CreateConfig(FlyName, SpeedDamageCoefficientName, 0.7f, "");
            stompProcCoefficient = CreateConfig(FlyName, ProcCoefficientName, 1f, "");
            stompBaseRadius = CreateConfig(FlyName, RadiusName, 9f, "");
            stompVelocityRadiusMultiplier = CreateConfig(FlyName, SpeedRadiusName, 0.3f, "");
            stompForce = CreateConfig(FlyName, ForceName, 100f, "");
            stompFalloff = CreateConfig(FlyName, BlastFalloffName, BlastAttack.FalloffModel.Linear, "");
            baseFlyVectorSmoothTime.SettingChanged += OnConfigChanged;
            baseSpeedMultiplier.SettingChanged += OnConfigChanged;
            baseSpeedSmoothTime.SettingChanged += OnConfigChanged;
            groundPush.SettingChanged += OnConfigChanged;
            stompBaseDamageCoefficient.SettingChanged += OnConfigChanged;
            stompVelocityDamageCoefficient.SettingChanged += OnConfigChanged;
            stompProcCoefficient.SettingChanged += OnConfigChanged;
            stompBaseRadius.SettingChanged += OnConfigChanged;
            stompVelocityRadiusMultiplier.SettingChanged += OnConfigChanged;
            stompForce.SettingChanged += OnConfigChanged;
            stompFalloff.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitFly();
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
            damageCoefficient = CreateConfig(LaserName, DamageCoefficientName, 1f, "");
            procCoefficient = CreateConfig(LaserName, ProcCoefficientName, 1f, "");
            hitInterval = CreateConfig(LaserName, "Hit Interval", 0.1f, "");
            force = CreateConfig(LaserName, ForceName, 0f, "");
            range = CreateConfig(LaserName, RangeName, 512f, "");
            radius = CreateConfig(LaserName, RadiusName, 1.5f, "");
            damageCoefficient.SettingChanged += OnConfigChanged;
            procCoefficient.SettingChanged += OnConfigChanged;
            hitInterval.SettingChanged += OnConfigChanged;
            force.SettingChanged += OnConfigChanged;
            range.SettingChanged += OnConfigChanged;
            radius.SettingChanged += OnConfigChanged;
        }
        private static void OnConfigChanged(object sender, EventArgs e) => Language.InitLaser();
        public static ConfigEntry<float> damageCoefficient;
        public static ConfigEntry<float> procCoefficient;
        public static ConfigEntry<float> force;
        public static ConfigEntry<float> range;
        public static ConfigEntry<float> radius;
        public static ConfigEntry<float> hitInterval;
    }
}
