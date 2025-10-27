using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskOfOptions.Components.Panel;
using RoR2;
using RoR2.Navigation;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using static Demolisher.Content;
using static R2API.DotAPI;

namespace Demolisher
{
    public class Keywords
    {
        public const string MediumMeleeAttackName = "Medium Melee Attack";
        public const string FireGrenadeName = "Fire Grenade";
        public const string ShieldChargeName = "Shield Charge";
        public const string DamageCoefficientName = "Damage Coefficient";
        public const string ProcCoefficientName = "Proc Coefficient";
        public const string ForceName = "Force";
        public const string DurationName = "Duration";
        public const string MaxChargeName = "Max Charge";
        public const string AttackDurationName = "Attack Duration";
        public const string RadiusName = "Radius";
        public const string DistanceName = "Distance";
    }
    public static class Utils
    {
        
        public static DotController.DotDef CreateDOT(BuffDef buffDef, out DotController.DotIndex dotIndex, bool resetTimerOnAdd, float interval, float damageCoefficient, DamageColorIndex damageColorIndex, CustomDotBehaviour customDotBehaviour, CustomDotVisual customDotVisual = null, CustomDotDamageEvaluation customDotDamageEvaluation = null)
        {
            DotController.DotDef dotDef = new DotController.DotDef
            {
                resetTimerOnAdd = resetTimerOnAdd,
                interval = interval,
                damageCoefficient = damageCoefficient,
                damageColorIndex = damageColorIndex,
                associatedBuff = buffDef
            };
            dotIndex = DotAPI.RegisterDotDef(dotDef, customDotBehaviour, customDotVisual, customDotDamageEvaluation);
            return dotDef;

        }
        public static ConfigEntry<T> CreateConfig<T>(string section, string key, T defaultValue, string description)
        {
            return CreateConfig(Main.configFile, section, key, defaultValue, description);
        }
        public static ConfigEntry<T> CreateConfig<T>(ConfigFile configFile, string section, string key, T defaultValue, string description)
        {
            ConfigEntry<T> entry = configFile.Bind(section, key, defaultValue, description);
            if (Main.riskOfOptionsEnabled) ModCompatabilities.RiskOfOptionsCompatability.AddConfig(entry);
            return entry;
        }
        public static string GetInScenePath(Transform transform)
        {
            if (transform == null) return "null";
            var current = transform;
            var inScenePath = new List<string> { current.name };
            while (current != transform.root)
            {
                current = current.parent;
                inScenePath.Add(current.name);
            }
            var sb = new StringBuilder();
            foreach (var item in Enumerable.Reverse(inScenePath)) sb.Append($"/{item}");
            return sb.ToString().TrimStart('/');
        }
        public static InputBankTest.ButtonState GetButtonStateFromId(InputBankTest inputBankTest, int id)
        {
            if (id == 1) return inputBankTest.skill1;
            if (id == 2) return inputBankTest.skill2;
            if (id == 3) return inputBankTest.skill3;
            if (id == 4) return inputBankTest.skill4;
            return inputBankTest.skill1;
        }
    }
    public static class Extensions
    {
        public static GameObject RegisterCharacterBody(this GameObject characterBody, Action<GameObject> onCharacterBodyRegistered = null)
        {
            bodies.Add(characterBody);
            onCharacterBodyRegistered?.Invoke(characterBody);
            return characterBody;
        }
        public static GameObject RegisterCharacterMaster(this GameObject characterMaster, Action<GameObject> onCharacterMasterRegistered = null)
        {
            masters.Add(characterMaster);
            onCharacterMasterRegistered?.Invoke(characterMaster);
            return characterMaster;
        }
        public static T RegisterSkillDef<T>(this T skillDef, Action<T> onSkillDefRegistered = null) where T : SkillDef
        {
            skills.Add(skillDef);
            onSkillDefRegistered?.Invoke(skillDef);
            return skillDef;
        }
        public static T RegisterSkillFamily<T>(this T skillFamily, Action<T> onSkillFamilyRegistered = null) where T : SkillFamily
        {
            skillFamilies.Add(skillFamily);
            onSkillFamilyRegistered?.Invoke(skillFamily);
            return skillFamily;
        }
        public static T RegisterSurvivorDef<T>(this T survivorDef, Action<T> onSurvivorDefRegistered = null) where T : SurvivorDef
        {
            survivors.Add(survivorDef);
            onSurvivorDefRegistered?.Invoke(survivorDef);
            return survivorDef;
        }
        public static T RegisterSkinDef<T>(this T skinDef, Action<T> onSkinDefRegistered = null) where T : SkinDef
        {
            onSkinDefRegistered?.Invoke(skinDef);
            return skinDef;
        }
        public static T RegisterBuffDef<T>(this T buffDef, Action<T> onBuffDefRegistered = null) where T : BuffDef
        {
            buffs.Add(buffDef);
            onBuffDefRegistered?.Invoke(buffDef);
            return buffDef;
        }
        public static GameObject RegisterProjectile(this GameObject projectile, Action<GameObject> onProjectileRegistered = null)
        {
            projectiles.Add(projectile);
            networkPrefabs.Add(projectile);
            onProjectileRegistered?.Invoke(projectile);
            return projectile;
        }
        public static EffectDef RegisterEffect(this GameObject effect, Action<GameObject> onEffectRegistered = null)
        {
            EffectDef effectDef = new EffectDef
            {
                prefab = effect
            };
            effects.Add(effectDef);
            onEffectRegistered?.Invoke(effect);
            return effectDef;
        }
        public static GameObject RegisterNetworkPrefab(this GameObject networkPrefab, Action<GameObject> onnetworkPrefabRegistered = null)
        {
            networkPrefabs.Add(networkPrefab);
            return networkPrefab;
        }
        public static Type RegisterEntityState(this Type type)
        {
            states.Add(type);
            return type;
        }
        public static T2 RegisterWeapon<T1, T2>(this T1 skillDef, DemolisherWeaponDef.ModificationDelegate oneTimeModification, DemolisherWeaponDef.ModificationDelegate attackModification) where T1 : DemolisherWeaponSkillDef where T2 : DemolisherWeaponDef
        {
            T2 t2 = skillDef.demolisherWeaponDef as T2;
            return t2.RegisterWeapon(oneTimeModification, attackModification);
        }
        public static T RegisterWeapon<T>(this T weapon, DemolisherWeaponDef.ModificationDelegate oneTimeModification, DemolisherWeaponDef.ModificationDelegate attackModification) where T : DemolisherWeaponDef
        {
            weapon.oneTimeModification = oneTimeModification;
            weapon.attackModification = attackModification;
            return weapon;
        }
        public static T ModifySkill<T>(this T skillDef, string reloadSoundEvent, int bonusStockMultiplier) where T : SkillDef
        {
            skillDef.SetCustomCooldownRefreshSound(reloadSoundEvent);
            skillDef.SetBonusStockMultiplier(bonusStockMultiplier);
            return skillDef;
        }
        public static void Overheal(this HealthComponent healthComponent, float amount) => healthComponent.Heal(amount, Assets.SoftnessProc);
        public static void OverhealFraction(this HealthComponent healthComponent, float percentage) => healthComponent.HealFraction(percentage, Assets.SoftnessProc);
        public static InputBankTest.ButtonState GetButtonStateFromId(this InputBankTest inputBankTest, int id) => Utils.GetButtonStateFromId(inputBankTest, id);
    }
}
