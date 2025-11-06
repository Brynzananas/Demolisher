using IL.RoR2.UI;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using static Demolisher.Keywords;

namespace Demolisher
{
    public static class Language
    {
        public static void Init()
        {
            InitCharacter();
            InitSharpness();
            InitSoftness();
            InitChaos();
            InitMediumMelee();
            InitShieldBash();
            InitChainDash();
            InitBoots();
            InitGrenadeLauncher();
            InitBombLauncher();
            InitHookLauncher();
            InitStickyLauncher();
            InitDemolisherLauncher();
            InitSwordPillar();
            InitParry();
            InitDetonate();
            InitWhirlwind();
            InitSlicing();
            InitCollapse();
            InitLaser();
            InitFly();
        }
        public static void InitCharacter()
        {
            AddLanguageToken(Assets.DemolisherCharacterBody.baseNameToken, "Demolisher");
            AddLanguageToken(Assets.DemolisherCharacterBody.subtitleNameToken, "Reborn Demon");
            AddLanguageToken(Assets.Demolisher.displayNameToken, "Demolisher");
            AddLanguageToken(Assets.Demolisher.descriptionToken, "ugh");
            AddLanguageToken(Assets.Demolisher.mainEndingEscapeFailureFlavorToken, "...and so he left, escaping eternal torment");
            AddLanguageToken(Assets.Demolisher.outroFlavorToken, "...and so he vanished, leaving nothing behind");
        }
        public static void InitSharpness()
        {
            AddLanguageToken(Assets.Sharpness.skillNameToken, SharpnessName);
            AddLanguageToken(Assets.Sharpness.skillDescriptionToken, $"First melee hit deals {damagePrefix}{Hooks.SharpnessDamageMultiplier * 100f}% more damage{endPrefix}. Each melee hit increases melee attack crit chance by {damagePrefix}{Hooks.SharpnessCritAddition}%{endPrefix} that resets on crit.");
        }
        public static void InitSoftness()
        {
            AddLanguageToken(Assets.Softness.skillNameToken, SoftnessName);
            AddLanguageToken(Assets.Softness.skillDescriptionToken, $"Each melee hit heals you for {healingPrefix}{Hooks.SoftnessHealOnHitPercentage}% max health{endPrefix}.\nEach melee kill heals you for {healingPrefix}{Hooks.SoftnessHealOnKillPercentage}% max health{endPrefix}.");
        }
        public static void InitChaos()
        {
            AddLanguageToken(Assets.Chaos.skillNameToken, ChaosName);
            AddLanguageToken(Assets.Chaos.skillDescriptionToken, $"On melee hit create an explosion that deals {damagePrefix}{Hooks.ChaosDamageCoefficient * 100f}% base damage{endPrefix}. Reharges after {Hooks.ChaosCooldown} seconds.");
        }
        public static void InitMediumMelee()
        {
            AddLanguageToken(Assets.MediumMelee.skillNameToken, MediumMeleeAttackName);
            AddLanguageToken(Assets.MediumMelee.skillDescriptionToken, $"Swing in the direction you are looking for {damagePrefix}{MediumMeleeAttack.damageCoefficient * 100f}% base damage{endPrefix}");
        }
        public static void InitShieldBash()
        {
            AddLanguageToken(Assets.ShieldBash.skillNameToken, ShieldChargeName);
            AddLanguageToken(Assets.ShieldBash.skillDescriptionToken, $"Charge forward for {ShieldCharge.baseDuration} seconds, bashing though enemies dealing {damagePrefix}{ShieldCharge.shieldBashDamageCoefficient * 100f}% base damage{endPrefix}.");
        }
        public static void InitChainDash()
        {
            AddLanguageToken(Assets.ChainDash.skillNameToken, ChainDashName);
            AddLanguageToken(Assets.ChainDash.skillDescriptionToken, $"Dash to inputted direction. Press skill button between {ChainDash.baseStartWindow} and {ChainDash.baseEndWindow} seconds to chain dash. Succesfull chain dash will reset current melle attack");
        }
        public static void InitBoots()
        {
            AddLanguageToken(Assets.Boots.skillNameToken, BootsName);
            AddLanguageToken(Assets.Boots.skillDescriptionToken, $"{damagePrefix}Heavy{endPrefix}. Negates fall damage. Landing with enough velocity will create an explosion, dealing {damagePrefix}{Hooks.stompBaseDamageCoefficient * 100f}% base damage{endPrefix}.");
        }
        public static void InitGrenadeLauncher()
        {
            AddLanguageToken(Assets.GrenadeLauncher.skillNameToken, "Impact Grenade");
            AddLanguageToken(Assets.GrenadeLauncher.skillDescriptionToken, $"Fire grenade that explodes on impact for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.GrenadeLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}.");
        }
        public static void InitBombLauncher()
        {
            AddLanguageToken(Assets.BombLauncher.skillNameToken, "Heavy Bomb");
            AddLanguageToken(Assets.BombLauncher.skillDescriptionToken, $"Fire bomb that explodes after time and enemy collision for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.BombLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}. Hold down skill button to reduce detonation time.");
        }
        public static void InitHookLauncher()
        {
            AddLanguageToken(Assets.HookLauncher.skillNameToken, "Grappling Hook");
            AddLanguageToken(Assets.HookLauncher.skillDescriptionToken, $"Fire hook that moves hit enemies around or pulls attacker on terrain hit.");
        }
        public static void InitStickyLauncher()
        {
            AddLanguageToken(Assets.StickyLauncher.skillNameToken, "Sticky Trap");
            AddLanguageToken(Assets.StickyLauncher.skillDescriptionToken, $"Fire trap that explodes on remote detonation for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.StickyLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}.");
        }
        public static void InitDemolisherLauncher()
        {
            AddLanguageToken(Assets.DemolisherLauncher.skillNameToken, "Demolisher");
            AddLanguageToken(Assets.DemolisherLauncher.skillDescriptionToken, $"Fire Demolisher that explodes on impact for {damagePrefix}{FireGrenadeConfig.damageCoefficient.Value * (Assets.DemolisherLauncher.demolisherWeaponDef as DemolisherProjectileWeaponDef).damageMultiplier * 100f}% base damage{endPrefix}.");
        }
        public static void InitSwordPillar()
        {
            AddLanguageToken(Assets.SwordPillar.skillNameToken, FireTallSwordName);
            AddLanguageToken(Assets.SwordPillar.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Fire tall sword projection that slices through for {damagePrefix}{FireTallSword.damageCoefficient * 100f}% base damage{endPrefix}.");
        }
        public static void InitParry()
        {
            AddLanguageToken(Assets.Parry.skillNameToken, ParryName);
            AddLanguageToken(Assets.Parry.skillDescriptionToken, $"Parry an incoming attack and create and explosion that deals {damagePrefix}{Parry.damageCoefficient * 100f}% base damage{endPrefix} on succesfull parry.");
        }
        public static void InitDetonate()
        {
            AddLanguageToken(Assets.Detonate.skillNameToken, "Detonate Traps");
            AddLanguageToken(Assets.Detonate.skillDescriptionToken, $"Detonates all placed traps.");
        }
        public static void InitWhirlwind()
        {
            AddLanguageToken(Assets.Whirlwind.skillNameToken, WhirlwindMeleeName);
            AddLanguageToken(Assets.Whirlwind.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Hold to spin for {damagePrefix}{WhirlwindMelee.damageCoefficient * WhirlwindMelee.baseRotationsPerSecond * 100f}% base damage{endPrefix} per second.");
        }
        public static void InitSlicing()
        {
            AddLanguageToken(Assets.Slicing.skillNameToken, SlicingName);
            AddLanguageToken(Assets.Slicing.skillDescriptionToken, $"{damagePrefix}Melee{endPrefix}. Stop time and enter slicing state. Press primary attack to slice through enemies for {damagePrefix}{Slicing.damageCoefficient * 100f}% base damage{endPrefix}.");
        }
        public static void InitCollapse()
        {
            AddLanguageToken(Assets.Collapse.skillNameToken, CollapseName);
            AddLanguageToken(Assets.Collapse.skillDescriptionToken, $"Fire beam of collapse that explodes for {damagePrefix}{FireCollapse.explosionDamageCoefficient * 100f}% base damage{endPrefix}.");
        }
        public static void InitLaser()
        {
            AddLanguageToken(Assets.Laser.skillNameToken, LaserName);
            AddLanguageToken(Assets.Laser.skillDescriptionToken, $"Hold to fire beam of pressure for {damagePrefix}{Laser.damageCoefficient * (1f / Laser.hitInterval) * 100f}% base damage{endPrefix} per second.");
        }
        public static void InitFly()
        {
            AddLanguageToken(Assets.Fly.skillNameToken, FlyName);
            AddLanguageToken(Assets.Fly.skillDescriptionToken, $"{damagePrefix}Heavy{endPrefix}. Turn into a missile, dealing {damagePrefix}{Fly.stompBaseDamageCoefficient * 100f}% base damage{endPrefix} on impact.");
        }
        public static void AddLanguageToken(string token, string text) => AddLanguageToken(token, text, "en");
        public static void AddLanguageToken(string token, string text, string lang)
        {
            RoR2.Language language = RoR2.Language.languagesByName[lang];
            if (language == null) return;
            if (language.stringsByToken.ContainsKey(token))
            {
                language.stringsByToken[token] = text;
            }
            else
            {
                language.stringsByToken.Add(token, text);
            }
        }
        public const string damagePrefix = "<style=cIsDamage>";
        public const string keywordPrefix = "<style=cKeywordName>";
        public const string subPrefix = "<style=cSub>";
        public const string stackPrefix = "<style=cStack>";
        public const string utilityPrefix = "<style=cIsUtility>";
        public const string healingPrefix = "<style=cIsHealing>";
        public const string endPrefix = "</style>";
    }
    
}
