using BrynzaAPI;
using EntityStates;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using static Demolisher.Content;
using static R2API.SkinSkillVariants;
using static BrynzaAPI.BrynzaAPI;

namespace Demolisher
{
    public static class Assets
    {
        public static AssetBundle assetBundle;
        //public static AssetBundle assetBundle2;
        public static GameObject DemolisherBody;
        public static GameObject DemolisherMaster;
        public static GameObject DemolisherEmote;
        public static GameObject DemolisherElevator;
        public static SurvivorDef Demolisher;
        public static GameObject Slash;
        public static GameObject FeetEffect;
        public static GameObject DevilFeetEffect;
        public static GameObject TimestopEffect;
        public static GameObject CracksTrailEffect;
        public static GameObject PillarEffect;
        public static GameObject PillarExplosionEffect;
        public static GameObject LaserEffect;
        public static GameObject IamRedAsFuck;
        public static EffectDef DemolisherTracer;
        public static EffectDef CollapseExplosion;
        public static EffectDef ChainsExplosion;
        public static EffectDef ParryEffect;
        public static EffectDef Explosion;
        public static EffectDef DoubleDonk;
        public static EffectDef Rings;
        public static EffectDef Trail;
        public static GameObject LemurianFireBallGhost;
        public static GameObject GrenadeProjectile;
        public static GameObject StickyProjectile;
        public static GameObject HookProjectile;
        public static GameObject SwordPillarProjectile;
        public static GameObject DemolisherProjectile;
        public static SteppedSkillDef MediumMelee;
        public static SkillDef ShieldBash;
        public static SkillDef ChainDash;
        public static DemolisherWeaponSkillDef Sharpness;
        public static DemolisherBulletAttackWeaponDef SharpnessWeapon;
        public static DemolisherWeaponSkillDef Softness;
        public static DemolisherBulletAttackWeaponDef SoftnessWeapon;
        public static DemolisherWeaponSkillDef Chaos;
        public static DemolisherBulletAttackWeaponDef ChaosWeapon;
        public static PassiveItemSkillDef Boots;
        public static DemolisherWeaponSkillDef GrenadeLauncher;
        public static DemolisherProjectileWeaponDef GrenadeWeapon;
        public static DemolisherWeaponSkillDef BombLauncher;
        public static DemolisherProjectileWeaponDef BombWeapon;
        public static DemolisherWeaponSkillDef HookLauncher;
        public static DemolisherProjectileWeaponDef HookWeapon;
        public static DemolisherWeaponSkillDef StickyLauncher;
        public static DemolisherProjectileWeaponDef StickyWeapon;
        public static SkillDef SwordPillar;
        public static SkillDef Parry;
        public static SkillDef Detonate;
        public static SkillDef Whirlwind;
        public static SkillDef Slicing;
        public static SkillDef Collapse;
        public static SkillDef Fly;
        public static SkillDef Laser;
        public static SkillFamily Passive;
        public static SkillFamily MeleeWeapon;
        public static SkillFamily MeleePrimary;
        public static SkillFamily MeleeSecondary;
        public static SkillFamily MeleeUtility;
        public static SkillFamily MeleeSpecial;
        public static SkillFamily RangedPrimary;
        public static SkillFamily RangedSecondary;
        public static SkillFamily RangedUtility;
        public static SkillFamily RangedSpecial;
        public static SkinDef Default;
        public static SkinDef Nuclear;
        public static BuffDef SharpnessCooldown;
        public static BuffDef SharpnessCritAddition;
        public static BuffDef ChaosCooldown;
        public static BuffDef BombHit;
        public static BuffDef InstantMeleeSwing;
        public static BuffDef IgnoreBoots;
        public static ItemDef BootsPassive;
        public static DamageAPI.ModdedDamageType SharpnessDamageType = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType SoftnessDamageType = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType ChaosDamageType = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType BombDamageType = DamageAPI.ReserveDamageType();
        public static RocketJumpDelegateDef AddFeetEffect;
        public static OnMasterSummonDelegateDef SetBodyStateToMainOnSpawn;
        public static SkinSkillVariantsDef skinSkillVariantsDef;
        public static SkinSkillVariantsDef skinNuclearSkillVariantsDef;
        public static PostProcessProfile TimestopPP;
        public static NetworkSoundEventDef ShieldBashSound;
        public static BodyIndex DemolisherBodyIndex;
        public static void Init()
        {
            assetBundle = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(global::Demolisher.DemolisherPlugin.PInfo.Location), "assetbundles", "demolisherassets")).assetBundle;
            //assetBundle2 = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Main.PInfo.Location), "assetbundles", "demolisherassets2")).assetBundle;
            SoundAPI.SoundBanks.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(global::Demolisher.DemolisherPlugin.PInfo.Location), "soundbanks", "Demoman.bnk"));
            foreach (Material material in assetBundle.LoadAllAssets<Material>())
            {
                if (!material.shader.name.StartsWith("StubbedRoR2"))
                {
                    continue;
                }

                string shaderName = material.shader.name.Replace("StubbedRoR2", "RoR2") + ".shader";
                Shader replacementShader = Addressables.LoadAssetAsync<Shader>(shaderName).WaitForCompletion();
                if (replacementShader)
                {
                    material.shader = replacementShader;
                }
            }
            foreach (VoicelineDef voicelineDef in assetBundle.LoadAllAssets<VoicelineDef>())
            {
                voicelineDef.Register();
            }
            DemolisherBody = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Character/DemolisherBody.prefab").RegisterCharacterBody();
            DemolisherMaster = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Character/DemolisherMonsterMaster.prefab").RegisterCharacterMaster();
            DemolisherElevator = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Elevator/DemolisherElevator.prefab").RegisterNetworkPrefab();
            BuffPassengerWhileSeated buffPassengerWhileSeated = DemolisherElevator.GetComponent<BuffPassengerWhileSeated>();
            buffPassengerWhileSeated.buff = RoR2Content.Buffs.HiddenInvincibility;
            DemolisherEmote = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Character/DemolisherEmotes.prefab");
            if (global::Demolisher.DemolisherPlugin.emotesEnabled) ModCompatabilities.EmoteCompatability.Init();
            GenericSkill[] genericSkills = DemolisherBody.GetComponents<GenericSkill>();
            foreach (GenericSkill genericSkill in genericSkills)
            {
                if (genericSkill.skillName.Contains("Melee")) genericSkill.SetSection("Melee");
                if (genericSkill.skillName.Contains("Ranged")) genericSkill.SetSection("Ranged");
                if (genericSkill.skillName.Contains("Primary")) genericSkill.SetLoadoutTitleTokenOverride("LOADOUT_SKILL_PRIMARY");
                if (genericSkill.skillName.Contains("Secondary")) genericSkill.SetLoadoutTitleTokenOverride("LOADOUT_SKILL_SECONDARY");
                if (genericSkill.skillName.Contains("Utility")) genericSkill.SetLoadoutTitleTokenOverride("LOADOUT_SKILL_UTILITY");
                if (genericSkill.skillName.Contains("Special")) genericSkill.SetLoadoutTitleTokenOverride("LOADOUT_SKILL_SPECIAL");
            }
            CameraTargetParams cameraTargetParams = DemolisherBody.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandard.asset").WaitForCompletion();
            CharacterBody demolisherCharacterBody = DemolisherBody.GetComponent<CharacterBody>();
            demolisherCharacterBody.preferredPodPrefab = DemolisherElevator; //LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod");//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/SurvivorPod.prefab").WaitForCompletion();
            demolisherCharacterBody._defaultCrosshairPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/SimpleDotCrosshair");// Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            GameObject gameObject = DemolisherBody.GetComponent<ModelLocator>().modelTransform.gameObject;
            gameObject.GetComponent<FootstepHandler>().footstepDustPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/GenericFootstepDust");//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion();
            ModelSkinController modelSkinController = gameObject.GetComponent<ModelSkinController>();
            if (modelSkinController)
            {
                foreach (SkinDef skinDef in modelSkinController.skins)
                {
                    SkinDefParams skinDefParams = skinDef.skinDefParams;
                    for (int i = 0; i < skinDefParams.rendererInfos.Length; i++)
                    {
                        ref CharacterModel.RendererInfo rendererInfo = ref skinDefParams.rendererInfos[i];
                        if (rendererInfo.renderer.name.Contains("Devil")) rendererInfo.SetDontFadeWhenNearCamera(true);
                        if (rendererInfo.defaultMaterial.name.Contains("Addressable"))
                        {
                            string key = rendererInfo.defaultMaterial.name.Replace("Addressable", "") + ".mat";
                            while (key.Contains("%"))
                            {
                                key = key.Replace("%", "/");
                            }
                            Material material = Addressables.LoadAssetAsync<Material>(key).WaitForCompletion();
                            rendererInfo.defaultMaterial = material;
                        }
                    }
                }
            }
            Demolisher = assetBundle.LoadAsset<SurvivorDef>("Assets/Demolisher/Character/Demolisher.asset").RegisterSurvivorDef();
            Explosion = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherExplosion.prefab").RegisterEffect();
            DoubleDonk = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherDoubleDonk.prefab").RegisterEffect();
            CollapseExplosion = assetBundle.LoadAsset<GameObject>("Assets/Demoman/DemoNukeExplosion.prefab").RegisterEffect();
            ChainsExplosion = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherChainsExplosion.prefab").RegisterEffect();
            ParryEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherParry.prefab").RegisterEffect();
            Rings = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherRings.prefab").RegisterEffect();
            Trail = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherTrailEffect.prefab").RegisterEffect();
            DemolisherTracer = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherTracer.prefab").RegisterEffect();
            GrenadeProjectile = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Projectiles/GrenadeProjectile.prefab").RegisterProjectile();
            StickyProjectile = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Projectiles/StickyProjectile.prefab").RegisterProjectile();
            HookProjectile = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Projectiles/HookProjectile.prefab").RegisterProjectile();
            SwordPillarProjectile = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Projectiles/SwordPillar.prefab").RegisterProjectile();
            DemolisherProjectile = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Projectiles/DemolisherProjectile.prefab").RegisterProjectile();
            LemurianFireBallGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FireballGhost.prefab").WaitForCompletion();
            MediumMelee = assetBundle.LoadAsset<SteppedSkillDef>("Assets/Demolisher/SkillDefs/MeleePrimary/DemolisherMediumMelee.asset").RegisterSkillDef();
            ShieldBash = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeUtility/DemolisherShieldBash.asset").RegisterSkillDef();
            ChainDash = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeUtility/DemolisherChainDash.asset").RegisterSkillDef();
            Sharpness = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/MeleeWeapon/DemolisherSharpness.asset").RegisterSkillDef();
            Softness = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/MeleeWeapon/DemolisherSoftness.asset").RegisterSkillDef();
            Chaos = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/MeleeWeapon/DemolisherChaos.asset").RegisterSkillDef();
            Boots = assetBundle.LoadAsset<PassiveItemSkillDef>("Assets/Demolisher/SkillDefs/Passive/DemolisherHellSupport.asset").RegisterSkillDef();
            GrenadeLauncher = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/RangedSecondary/DemolisherGrenadeLauncher.asset").RegisterSkillDef();
            GrenadeLauncher.ModifySkill("Play_grenade_launcher_worldreload", GrenadeLauncher.baseMaxStock);
            BombLauncher = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/RangedSecondary/DemolisherBombLauncher.asset").RegisterSkillDef();
            BombLauncher.ModifySkill("Play_grenade_launcher_worldreload", BombLauncher.baseMaxStock);
            HookLauncher = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/RangedSecondary/DemolisherHookLauncher.asset").RegisterSkillDef();
            HookLauncher.ModifySkill("Play_grenade_launcher_worldreload", HookLauncher.baseMaxStock);
            SwordPillar = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeSecondary/DemolisherSwordPillar.asset").RegisterSkillDef();
            Parry = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeSecondary/DemolisherParry.asset").RegisterSkillDef();
            StickyLauncher = assetBundle.LoadAsset<DemolisherWeaponSkillDef>("Assets/Demolisher/SkillDefs/RangedPrimary/DemolisherStickyLauncher.asset").RegisterSkillDef();
            StickyLauncher.ModifySkill("Play_stickybomblauncher_worldreload", StickyLauncher.baseMaxStock);
            Detonate = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/RangedUtility/DemolisherDetonate.asset").RegisterSkillDef();
            Whirlwind = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeSpecial/DemolisherWhirlwind.asset").RegisterSkillDef();
            Slicing = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeSpecial/DemolisherSlicing.asset").RegisterSkillDef();
            Collapse = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/RangedSpecial/DemolisherCollapse.asset").RegisterSkillDef();
            Fly = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/MeleeSpecial/DemolisherFly.asset").RegisterSkillDef();
            Laser = assetBundle.LoadAsset<SkillDef>("Assets/Demolisher/SkillDefs/RangedSpecial/DemolisherLaser.asset").RegisterSkillDef();
            Passive = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherPassive.asset").RegisterSkillFamily();
            MeleeWeapon = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherMeleeWeapon.asset").RegisterSkillFamily();
            MeleePrimary = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherMeleePrimary.asset").RegisterSkillFamily();
            MeleeUtility = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherMeleeUtility.asset").RegisterSkillFamily();
            RangedPrimary = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherRangedPrimary.asset").RegisterSkillFamily();
            RangedUtility = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherRangedUtility.asset").RegisterSkillFamily();
            RangedSecondary = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherRangedSecondary.asset").RegisterSkillFamily();
            MeleeSecondary = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherMeleeSecondary.asset").RegisterSkillFamily();
            RangedSpecial = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherRangedSpecial.asset").RegisterSkillFamily();
            MeleeSpecial = assetBundle.LoadAsset<SkillFamily>("Assets/Demolisher/SkillFamilies/DemolisherMeleeSpecial.asset").RegisterSkillFamily();
            SharpnessCritAddition = assetBundle.LoadAsset<BuffDef>("Assets/Demolisher/Buffs/SharpnessCritAddition.asset").RegisterBuffDef();
            SharpnessCooldown = assetBundle.LoadAsset<BuffDef>("Assets/Demolisher/Buffs/SharpnessCooldown.asset").RegisterBuffDef();
            ChaosCooldown = assetBundle.LoadAsset<BuffDef>("Assets/Demolisher/Buffs/ChaosCooldown.asset").RegisterBuffDef();
            BombHit = assetBundle.LoadAsset<BuffDef>("Assets/Demolisher/Buffs/BombHit.asset").RegisterBuffDef();
            InstantMeleeSwing = assetBundle.LoadAsset<BuffDef>("Assets/Demolisher/Buffs/InstantMeleeSwing.asset").RegisterBuffDef();
            IgnoreBoots = assetBundle.LoadAsset<BuffDef>("Assets/Demolisher/Buffs/IgnoreBoots.asset").RegisterBuffDef();
            BootsPassive = assetBundle.LoadAsset<ItemDef>("Assets/Demolisher/Items/BootsPassive.asset").RegisterItemDef();
            Default = assetBundle.LoadAsset<SkinDef>("Assets/Demolisher/Character/DemolisherDefault.asset");
            Nuclear = assetBundle.LoadAsset<SkinDef>("Assets/Demolisher/Character/DemolisherNuclear.asset");
            //TimestopPP = assetBundle2.LoadAsset<PostProcessProfile>("Assets/Demolisher/DemolisherTimestopPP.asset");
            TimestopEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherTimestopEffect.prefab");
            //PostProcessVolume postProcessVolume = TimestopEffect.GetComponent<PostProcessVolume>();
            //postProcessVolume.sharedProfile = TimestopPP;
            skinSkillVariantsDef = assetBundle.LoadAsset<SkinSkillVariantsDef>("Assets/Demolisher/Character/DemolisherDefaultSkillVariants.asset");
            skinSkillVariantsDef.Register();
            skinNuclearSkillVariantsDef = assetBundle.LoadAsset<SkinSkillVariantsDef>("Assets/Demolisher/Character/DemolisherNuclearSkillVariants.asset");
            skinNuclearSkillVariantsDef.Register();
            Slash = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherSlash.prefab");
            FeetEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherFeetSmoke.prefab");
            CracksTrailEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherCracks.prefab");
            PillarEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherPillar.prefab");
            PillarExplosionEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherPillarExplosion.prefab");
            LaserEffect = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/DemolisherLaser.prefab");
            IamRedAsFuck = assetBundle.LoadAsset<GameObject>("Assets/Demolisher/Effects/IAmRedAsFuck.prefab");
            SharpnessWeapon = Sharpness.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherBulletAttackWeaponDef>(null, null);
            SharpnessWeapon.moddedDamageTypes = [SharpnessDamageType];
            SoftnessWeapon = Softness.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherBulletAttackWeaponDef>(null, null);
            SoftnessWeapon.moddedDamageTypes = [SoftnessDamageType];
            ChaosWeapon = Chaos.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherBulletAttackWeaponDef>(null, null);
            ChaosWeapon.moddedDamageTypes = [ChaosDamageType];
            GrenadeWeapon = GrenadeLauncher.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherProjectileWeaponDef>(null, null);
            BombWeapon = BombLauncher.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherProjectileWeaponDef>(null, null);
            BombWeapon.moddedDamageTypes = [BombDamageType];
            HookWeapon = HookLauncher.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherProjectileWeaponDef>(null, null);
            StickyWeapon = StickyLauncher.RegisterWeapon<DemolisherWeaponSkillDef, DemolisherProjectileWeaponDef>(null, null);
            AddFeetEffect = assetBundle.LoadAsset<RocketJumpDelegateDef>("Assets/Demolisher/DelegateDefs/AddFeetEffect.asset");
            AddFeetEffect.@delegate = DemolisherFeetEffect.AddFeetSmoke;
            SetBodyStateToMainOnSpawn = assetBundle.LoadAsset<OnMasterSummonDelegateDef>("Assets/Demolisher/DelegateDefs/SetBodyStateToMainOnSpawn.asset");
            SetBodyStateToMainOnSpawn.@delegate = SetBodyStateToMain;
            typeof(DemolisherMainState).RegisterEntityState();
            typeof(FireGrenade).RegisterEntityState();
            typeof(FireGrenadeHold).RegisterEntityState();
            typeof(FireGrenadeAndHold).RegisterEntityState();
            //typeof(FireGrenadeNetwork).RegisterEntityState();
            //typeof(FireGrenadeHoldNetwork).RegisterEntityState();
            typeof(MediumMeleeAttack).RegisterEntityState();
            typeof(ShieldCharge).RegisterEntityState();
            typeof(Detonate).RegisterEntityState();
            typeof(WhirlwindMelee).RegisterEntityState();
            typeof(Parry).RegisterEntityState();
            typeof(ChargeCollapse).RegisterEntityState();
            typeof(FireCollapse).RegisterEntityState();
            typeof(FireTallSword).RegisterEntityState();
            ContentManager.collectContentPackProviders += (addContentPackProvider) => addContentPackProvider(new Content());
        }
        public static void AddLemurianProjectileGhost(GameObject projectile)
        {
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();
            if (projectileController)
            {
                if (projectileController.ghostPrefab == null) projectileController.ghostPrefab = LemurianFireBallGhost;
            }
        }
        public static void SetBodyStateToMain(IProjectileMasterSummon projectileMasterSummon, CharacterMaster characterMaster)
        {
            if (characterMaster == null) return;
            CharacterBody characterBody = characterMaster.GetBody();
            if (characterBody == null) return;
            EntityStateMachine entityStateMachine = characterBody.GetComponent<EntityStateMachine>();
            if (entityStateMachine == null) return;
            entityStateMachine.SetStateToMain();
        }
    }
}
